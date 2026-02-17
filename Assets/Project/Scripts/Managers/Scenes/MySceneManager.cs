using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// シーン遷移と現在のシーン状態を管理するシングルトンクラス
/// </summary>
public class MySceneManager:PersistentSingleton<MySceneManager>{
    public enum SceneType{
        TITLE,
        GAME,
        OTHER
    }
    public SceneType CurrentSceneType{get;private set;}=SceneType.TITLE;

    [Header("シーン名の設定")]
    public string titleSceneName="Title";
    public string gameSceneName="Game";

    protected override void Awake(){
        base.Awake();
        
        UpdateCurrentSceneType(SceneManager.GetActiveScene().name);
        
        //シーンロード時のイベント登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    public void StartGame(){
        SceneManager.LoadScene(gameSceneName);
        AudioManager.Ins.PlayBGM(AudioManager.Ins.gameBGMIndex);
    }

    /// <summary>
    /// コンティニュー
    /// </summary>
    public void Continue(){
        SceneManager.LoadScene(gameSceneName);
        Player.Ins.Start();//プレイヤの初期化
        GameManager.Ins.isGameOver=false;
        Debugger.Log("isGameOver"+GameManager.Ins.isGameOver);
    }
    
    //シーンをロードしたときに実行
    void OnSceneLoaded(Scene scene,LoadSceneMode mode){
        UpdateCurrentSceneType(scene.name);
        
        //ScoreManagerの取得
        GameManager.Ins.scoreManager=FindObjectOfType<ScoreManager>();
        
        //現在なにも選択していない
        UIManager.Ins.currentlySelectedButtonInfoIndex=-1;
    }

    /// <summary>
    /// 現在のシーンタイプを更新
    /// </summary>
    /// <param name="sceneName"></param>
    void UpdateCurrentSceneType(string sceneName){
        if (sceneName==titleSceneName)CurrentSceneType=SceneType.TITLE;
        else if (sceneName==gameSceneName)CurrentSceneType=SceneType.GAME;
        else CurrentSceneType=SceneType.OTHER;
    }
}