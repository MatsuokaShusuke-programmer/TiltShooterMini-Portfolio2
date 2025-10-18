using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager:PersistentSingleton<GameManager>{
#if UNITY_EDITOR
    public bool useDebugInput=true;//デバッグ用の操作にするか
#endif

    [System.NonSerialized]public GameObject gameOverPanel;//ゲームオーバ時に見せるパネル
    //ゲームオーバ時に出すスコアテキスト
    [System.NonSerialized]public TextMeshProUGUI gameOverScoreText;
    [System.NonSerialized]public Button continueButton;//コンティニューボタン
    [System.NonSerialized]public Button quitButton;//終了ボタン
    //スコアマネージャ
    [System.NonSerialized]public　ScoreManager scoreManager;

    [System.NonSerialized]public bool isGameOver=false;

    protected override void Awake(){
        base.Awake();

        scoreManager=FindObjectOfType<ScoreManager>();
    }

    void Update(){
        //右ボタンが押されていないときリターン
        if(!JoyConInputManager.Ins.isDownRight)return;


        //現在選択されているボタンを取得
        var currentButton
            =UIManager.Ins.
                buttonInfos[UIManager.Ins.currentlySelectedButtonInfoIndex];
        
        //現在のシーンタイプによって処理を変える
        switch(MySceneManager.Ins.CurrentSceneType){
            case MySceneManager.SceneType.TITLE:
                //現在選択されているボタンによって処理を変える
                switch(currentButton.type){
                    case UIManager.ButtonType.START:
                        UIManager.Ins.OnClickStartButton();
                        break;

                    case UIManager.ButtonType.QUIT:
                        UIManager.Ins.OnClickQuitButton();
                        break;

                    case UIManager.ButtonType.CONTINUE:
                        UIManager.Ins.OnClickContinueButton();
                        break;
                }
                
                break;
            
            case MySceneManager.SceneType.GAME:
                if(!isGameOver)break;//ゲームオーバでなければブレイク
                
                //現在選択されているボタンによって処理を変える
                switch(currentButton.type){
                    case UIManager.ButtonType.START:
                        UIManager.Ins.OnClickStartButton();
                        break;

                    case UIManager.ButtonType.QUIT:
                        UIManager.Ins.OnClickQuitButton();
                        break;

                    case UIManager.ButtonType.CONTINUE:
                        UIManager.Ins.OnClickContinueButton();
                        break;
                }
                
                break;
        }
    }

    public void GameOver(){
        if(isGameOver) return;//既にゲームオーバならリターン
        
        isGameOver=true;
        Debugger.Log("isGameOver"+isGameOver);
        gameOverPanel.SetActive(true);//アクティブ化

        //ゲームオーバー時に出すスコア更新
        gameOverScoreText.text=scoreManager.Score.ToString();
        
        //コンティニューボタンに関数を割り当てる
        continueButton.onClick.AddListener(UIManager.Ins.OnClickContinueButton);
        //QuitButtonに関数を割り当てる
        quitButton.onClick.AddListener(UIManager.Ins.OnClickQuitButton);
    }

    public ScoreManager GetScoreManager()=>scoreManager;

    public void Quit(){
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#else
        Application.Quit();
#endif
    }
}