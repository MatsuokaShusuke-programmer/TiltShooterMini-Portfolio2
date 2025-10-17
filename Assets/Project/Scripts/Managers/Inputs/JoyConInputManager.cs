using UnityEngine;

public class JoyConInputManager:PersistentSingleton<JoyConInputManager>{
    //スティックの状態
    public enum StickInfo{
        NORMAL,
        RIGHT,
        LEFT,
        UP,
        DOWN,
    }
    [System.NonSerialized]public StickInfo currentlySelectedStick;

    //スティックのデッドゾーン
    [SerializeField]float _stickDeadZone = 0.2f;
    
    //横持ちの傾き
    [System.NonSerialized]public Quaternion orientationDelta;
    //攻撃ボタンが押されているか(SL/SR)
    [System.NonSerialized]public bool isAttackButton=false;
    //横持ちの右ボタン
    [System.NonSerialized]public bool isDownRight=false;
    
    [System.NonSerialized]public Joycon joycon; //一番始めにつないだJoy-Con

    Quaternion _orientation;//Joy-Conの傾き
    //傾きの基準(横)
    readonly Quaternion _baseOrientation=Quaternion.Euler(-90f,90f,-90f);
    Quaternion _invBaseOrientation;// _baseOrientationの逆をキャッシュ

    float[] _stick;//スティック
    //横持ちのスティック
    public Vector2 HorizontalGripStickInput{private set;get;}

    void Start(){
        //Joy-Conが繋がっているとき
        if(JoyconManager.Instance.j.Count>0){
            joycon=JoyconManager.Instance.j[0];//Joy-Conの取得
        }
        
        //逆回転を取得
        _invBaseOrientation=Quaternion.Inverse(_baseOrientation);
    }

    void Update(){
        if (joycon==null) return;
        
        UpdateOrientation();
        UpdateButtons();
        UpdateStickInput();
    }

    //Joy-Conの傾きを更新
    void UpdateOrientation(){
        _orientation=joycon.GetVector();//傾き
        
        //Joy-Conを横に持った時の傾き
        orientationDelta=_invBaseOrientation*_orientation;
    }
    
    //Joy-Conのボタンの更新
    void UpdateButtons(){
        //SR/SLを押されてるとき攻撃
        isAttackButton
            =joycon.GetButton(Joycon.Button.SL)||
             joycon.GetButton(Joycon.Button.SR);
        
        //LかRかで処理を変える
        if(joycon.isLeft){
            //Lの横持ちで右ボタン
            isDownRight=joycon.GetButtonDown(Joycon.Button.DPAD_DOWN);
        }
        else{
            
            //Rの横持ちで右ボタン
            isDownRight=joycon.GetButtonDown(Joycon.Button.DPAD_UP);
        }
    }
    
    //Joy-Conのスティックの更新
    void UpdateStickInput(){
        _stick=joycon.GetStick();//スティックの取得
        //左か右かで処理を分ける
        if(joycon.isLeft){
            //左の横持ちのスティック
            HorizontalGripStickInput=new Vector2(_stick[0],-_stick[1]);
        }
        else{
            //右の横持ちのスティック
            HorizontalGripStickInput=new Vector2(_stick[1],-_stick[0]);
        }

        //スティックが倒されているか
        if (Mathf.Abs(HorizontalGripStickInput.y) > _stickDeadZone||
            Mathf.Abs(HorizontalGripStickInput.x )> _stickDeadZone){
            //垂直か水平どちらが大きいか、垂直優先
            //垂直の値が大きいとき
            if (Mathf.Abs(HorizontalGripStickInput.y)
                >= Mathf.Abs(HorizontalGripStickInput.x)){
                //上下判定
                currentlySelectedStick
                    =HorizontalGripStickInput.y > 0?StickInfo.UP:StickInfo.DOWN;
            }
            else{
                //左右判定
                currentlySelectedStick
                    =HorizontalGripStickInput.x>0?StickInfo.RIGHT:StickInfo.LEFT;
            }
        }
        else{
            //スティックは倒されていない
            currentlySelectedStick=StickInfo.NORMAL;
        }
    }
}