using UnityEngine;
using TMPro;

/// <summary>
/// 制限時間の管理を行うクラス
/// </summary>
public class TimeManager:PersistentSingleton<TimeManager>{
    [System.NonSerialized]public TextMeshProUGUI timeText;//制限時間のテキスト
    
    [SerializeField]float _timeLimit=15f;//制限時間
    float _currentTime;
    bool _isRunning=true;//タイマが実行中か

    protected override void Awake(){
        base.Awake();
        
        timeText=gameObject.GetComponent<TextMeshProUGUI>();//テキストの取得
        _currentTime=_timeLimit;//現在の制限時間を初期化
        UpdateTimeUI();
    }

    // Update is called once per frame
    void Update(){
        if(!_isRunning)return;//タイマが実行中でないときリターン
        
        _currentTime-=Time.deltaTime;//時間を減らす
        //時間切れのとき
        if (_currentTime <=0){
            _currentTime=0;//残り時間を0に
            _isRunning=false;
            UpdateTimeUI();
            GameManager.Ins.GameOver();
            return;
        }
        UpdateTimeUI();
    }

    //テキストを更新
    void UpdateTimeUI(){
        if (timeText) timeText.text=_currentTime.ToString("0.00");
    }
}