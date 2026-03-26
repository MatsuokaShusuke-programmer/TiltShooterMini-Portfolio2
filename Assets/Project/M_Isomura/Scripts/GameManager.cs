using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager:MonoBehaviour {
   
    /// <summary>
    /// ゲームモード
    /// </summary>
    public enum GameMode {
        Normal,
        Endless
    }

    public static GameManager Instance { get; private set; }

    //地球攻撃イベント
    public static Action OnEarthAttacked;

    //現在のモード
    public GameMode CurrentGameMode { get; private set; }
    public bool isPlaying;
    public int CurrentStageNum { get; private set; }

    private static PlayState playState = new PlayState();
    private static GameOverState gameOverState = new GameOverState();
    private static GameClearState gameClearState = new GameClearState();
    private static TitleState titleState = new TitleState();
    private GameStateBase currentState;
    private ResultDisplay resultDisplay;
    private void Awake() {
        //シングルトン化
        if(Instance!=null) Destroy(gameObject);
        else Instance=this;
        DontDestroyOnLoad(gameObject);


        currentState = titleState;
        currentState.OnEnter(this,null);
        isPlaying = true;
    }

    void Start() 
    {
        if(ResourceManager.Instance) {
            //イベントが発生したらGameOver()を呼ぶように登録
            ResourceManager.Instance.OnHPDepleted+=GameOver;
            ResourceManager.Instance.OnFuelDepleted+=GameOver;
            ResourceManager.Instance.OnOxygenDepleted+=GameOver;
        }

        if(EnemyManager.Ins) {
            EnemyManager.Ins.OnAllEnemiesDie+=GameClear;
        }
    }

    private void ChangeState(GameStateBase nextState) {
        currentState.OnExit(this,nextState);
        nextState.OnEnter(this,currentState);
        currentState=nextState;
    }

    // Update is called once per frame
    void Update() {
        currentState.OnUpDate(this);
    }

    void OnEnable() {
        SceneManager.sceneLoaded+=OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded-=OnSceneLoaded;
    }

    void OnDestroy() {
        //購読解除
        OnEarthAttacked-=HandleEarthAttacked;
        if(ResourceManager.Instance) {
            ResourceManager.Instance.OnHPDepleted-=GameOver;
            ResourceManager.Instance.OnFuelDepleted-=GameOver;
            ResourceManager.Instance.OnOxygenDepleted-=GameOver;
        }
    }

    /// <summary>
    /// ゲームの進行状態をリセット
    /// </summary>
    public void StateReset()
    {
        currentState = titleState;
    }

    /// <summary>
    /// シーンが切り替わった直後の処理
    /// </summary>
    void OnSceneLoaded(Scene scene,LoadSceneMode mode) {
        SubscribeToResourceManager();

        if (GameObject.Find("ResultCanvas") != null) resultDisplay = GameObject.Find("ResultCanvas").GetComponent<ResultDisplay>();

        //EnemyManagerがあるとき、以下のイベントを登録
        EnemyManager enemyManager = FindAnyObjectByType<EnemyManager>();
        if(enemyManager) {
            enemyManager.OnAllEnemiesDie-=GameClear;
            enemyManager.OnAllEnemiesDie+=GameClear;
        }

        //イベントを追加
        OnEarthAttacked += HandleEarthAttacked;
    }

    /// <summary>
    /// リソースマネージャーへのイベント登録
    /// </summary>
    void SubscribeToResourceManager() {
        if(ResourceManager.Instance) {
            // 二重登録を防ぐため、念のため一度解除
            ResourceManager.Instance.OnHPDepleted-=GameOver;
            ResourceManager.Instance.OnFuelDepleted-=GameOver;
            ResourceManager.Instance.OnOxygenDepleted-=GameOver;

            // 登録
            ResourceManager.Instance.OnHPDepleted+=GameOver;
            ResourceManager.Instance.OnFuelDepleted+=GameOver;
            ResourceManager.Instance.OnOxygenDepleted+=GameOver;
        }
    }

    /// <summary>
    /// ステージを選択してゲームを開始
    /// </summary>
    public void StartStage(int stageNum) {
        CurrentStageNum = stageNum;
        GameStart(GameMode.Normal);
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    /// <param name="mode">ゲームモード</param>
    public void GameStart(GameMode mode) {
        isPlaying = true;
        CurrentGameMode=mode;//モードを保持
        ChangeScene("InGame");
        ChangeState(playState);//プレイステートへ以降
    }

    /// <summary>
    /// ハンガーへ遷移
    /// </summary>
    public void GoToHangar() {
        ChangeScene("Hangar");
    }

    void GameOver() {
        if(currentState is GameOverState) return;
        isPlaying = false;
        resultDisplay.ResultRequest(false, 0, EquipmentManager.Ins.Inventory);
        ChangeState(gameOverState);
    }

    /// <summary>
    /// ゲームクリア
    /// </summary>
    void GameClear() {
        if(currentState is GameClearState||currentState is GameOverState) 
            return;
        isPlaying = false;
        //リザルト画面にクリアと伝える
        resultDisplay.ResultRequest(true,0, EquipmentManager.Ins.Inventory);
        ChangeState(gameClearState);
    }

    public void ChangeScene(int sceneNum) {
        SceneManager.LoadScene(sceneNum);
    }

    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 地球が攻撃されたときの処理
    /// </summary>
    void HandleEarthAttacked() {
        //すでにGameOverのあらreturn
        if(currentState is GameOverState) return;

        GameOver();
    }

    /// <summary>
    /// ゲーム終了関数
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //このコードでビルドしたゲームを終了することができる
        Application.Quit();
#endif
    }
}
