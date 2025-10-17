using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameOver画面UIの制御
/// </summary>
public class GameOverUI:MonoBehaviour{
    [SerializeField]GameObject _gameOverPanel;//ゲームオーバパネル
    //ゲームオーバ時に出すスコアのテキストボックス
    [SerializeField]TextMeshProUGUI _gameOverScoreText;
    [SerializeField]Button _continueButton; //コンティニューボタン
    [SerializeField]Button _quitButton;     //終了ボタン
    
    void Start(){
        //UIをGameManagerに渡す
        GameManager.Ins.gameOverPanel=_gameOverPanel;
        GameManager.Ins.gameOverScoreText=_gameOverScoreText;
        GameManager.Ins.continueButton=_continueButton;
        GameManager.Ins.quitButton=_quitButton;
        
        _gameOverPanel.SetActive(false);//初期状態では非表示
    }
}