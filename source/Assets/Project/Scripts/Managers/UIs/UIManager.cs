using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI全体の管理を行うシングルトン
/// </summary>
public class UIManager:PersistentSingleton<UIManager>{
    //ボタンの種類
    public enum ButtonType{
        START,
        QUIT,
        CONTINUE
    }

    //ボタンの情報
    [System.Serializable]
    public struct ButtonInfo{
        public Button button;
        public ButtonType type;
    }

    [System.NonSerialized]public ButtonInfo[] buttonInfos;
    //選択中のボタンインデックス(-1:未設定)
    [System.NonSerialized]public int currentlySelectedButtonInfoIndex=-1;

    //ボタンの通常色
    [SerializeField]Color _normalButtonColor=Color.white;
    //ボタンの選択色
    [SerializeField]Color _selectedButtonColor=Color.red;

    bool _canChangeSelectButton=true;//選択変更の可否

    /// <summary>
    /// UI初期化(選択状態の色設定)
    /// </summary>
    public void Init(){
        for (int i=0; i < buttonInfos.Length; i++){
            ButtonInfo bi=buttonInfos[i];
            //選択中かどうかで色を変える
            bi.button.image.color
                =i!=currentlySelectedButtonInfoIndex?_normalButtonColor:_selectedButtonColor;
        }
    }

    void Update(){
        //現在のシーンタイプによって処理を変える
        switch(MySceneManager.Ins.CurrentSceneType){
            case MySceneManager.SceneType.TITLE:
                //スティック入力によって処理を変える
                switch(JoyConInputManager.Ins.currentlySelectedStick){
                    case JoyConInputManager.StickInfo.NORMAL:
                        //選択を変更できるようにする
                        _canChangeSelectButton=true;
                        break;

                    case JoyConInputManager.StickInfo.UP:
                        BackButtonSelect();
                        break;

                    case JoyConInputManager.StickInfo.DOWN:
                        NextButtonSelect();
                        break;

                    case JoyConInputManager.StickInfo.RIGHT:
                        NextButtonSelect();
                        break;

                    case JoyConInputManager.StickInfo.LEFT:
                        BackButtonSelect();
                        break;
                }
                
                break;
            
            case MySceneManager.SceneType.GAME:
                if(!GameManager.Ins.isGameOver)break;//ゲームオーバでなければブレイク
                
                //スティック入力によって処理を変える
                switch(JoyConInputManager.Ins.currentlySelectedStick){
                    case JoyConInputManager.StickInfo.NORMAL:
                        //選択を変更できるようにする
                        _canChangeSelectButton=true;
                        break;

                    case JoyConInputManager.StickInfo.UP:
                        BackButtonSelect();
                        break;

                    case JoyConInputManager.StickInfo.DOWN:
                        NextButtonSelect();
                        break;

                    case JoyConInputManager.StickInfo.RIGHT:
                        NextButtonSelect();
                        break;

                    case JoyConInputManager.StickInfo.LEFT:
                        BackButtonSelect();
                        break;
                }
                
                break;
        }

    }

    /// <summary>
    /// 次のボタンを選択
    /// </summary>
    void NextButtonSelect(){
        if (!_canChangeSelectButton) return;//ボタンの選択変更できないときリターン

        //選択してたボタンの色をもとに戻す
        buttonInfos[currentlySelectedButtonInfoIndex].button.image.color
            =_normalButtonColor;

        //インデックスの更新
        currentlySelectedButtonInfoIndex++;
        if (currentlySelectedButtonInfoIndex > buttonInfos.Length-1)
            currentlySelectedButtonInfoIndex=0;

        //次のボタンを選択色に
        buttonInfos[currentlySelectedButtonInfoIndex].button.image.color
            =_selectedButtonColor;

        //一度選択変更したらスティックを戻すまで変更負荷
        _canChangeSelectButton=false;
    }

    /// <summary>
    /// 前のボタンを選択
    /// </summary>
    void BackButtonSelect(){
        if (!_canChangeSelectButton) return;//ボタンの選択変更不可のときリターン
        //選択してたボタンを通常色に戻す
        buttonInfos[currentlySelectedButtonInfoIndex].button.image.color
            =_normalButtonColor;

        //インデックスの更新
        currentlySelectedButtonInfoIndex--;
        if(currentlySelectedButtonInfoIndex<0)
            currentlySelectedButtonInfoIndex=buttonInfos.Length-1;

        //前のボタンを選択色に
        buttonInfos[currentlySelectedButtonInfoIndex].button.image.color
            =_selectedButtonColor;

        //選択を変更できないようにする
        _canChangeSelectButton=false;
    }

    //スタートボタン
    public void OnClickStartButton(){
        MySceneManager.Ins.StartGame();
    }

    //コンティニューボタン
    public void OnClickContinueButton(){
        MySceneManager.Ins.Continue();
    }

    //終了ボタン
    public void OnClickQuitButton(){
        GameManager.Ins.Quit();
    }
}