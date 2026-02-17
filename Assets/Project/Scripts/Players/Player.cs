using System.Collections;
using UnityEngine;

public class Player:PersistentSingleton<Player>{
    [Header("ステータス")]
    [SerializeField]int _hp=10;
    [SerializeField]Vector3 _pos=new Vector3(0f,0f,0f);
    
    //移動範囲の制限
    [SerializeField]float _minMovementRangeX=-10;
    [SerializeField]float _maxMovementRangeX=10;
    [SerializeField]float _minMovementRangeY=-5;
    [SerializeField]float _maxMovementRangeY=15;
    [SerializeField]float _minMovementRangeZ=-5;
    [SerializeField]float _maxMovementRangeZ=15;
    
    //移動スピード
    [SerializeField]float _tiltMoveSpeed=10f;
    [SerializeField]float _stickMoveSpeed=10f;
    
    [SerializeField]float _deadZone=0.1f;//Playerが動き始めるJoy-Conの傾きの最小値
    [SerializeField]float _tiltMultiplier=40f;//Playerを傾ける倍率
    [SerializeField]float _attackInterval=0.1f;//攻撃間隔
    
    [Header("ビーム")]
    [SerializeField]GameObject _beam;//ビーム
    [SerializeField]float _beamOffsetZ=1f;//どれだけプレイヤの前で生成するか

    Rigidbody _rb;
    Vector3 _moveVector;//Playerの移動ベクトル
    Vector3 _beamPos;//ビームを生成する座標

    bool _canAttack=true;//攻撃できるか
    
    protected override void Awake(){
        base.Awake();
        
        _rb = GetComponent<Rigidbody>();
    }

    public void Start(){
        transform.position = _pos;
    }

    void Update(){
#if UNITY_EDITOR
        if (GameManager.Ins.useDebugInput)DebugAttack();
        else Attack();
#else
        Attack();
#endif
    }

    void FixedUpdate(){
#if UNITY_EDITOR
        if (GameManager.Ins.useDebugInput)DebugMove();
        else Move();
#else
        Move();
#endif
    }

    /// <summary>
    /// ビームを発射
    /// </summary>
    void ShootBeam(){
        //ビームのスポーン場所をプレイヤの前にする
        _beamPos=transform.position;
        _beamPos.z+=_beamOffsetZ;
        //ビーム生成
        Instantiate(_beam,_beamPos,Quaternion.identity);
            
        //発射音再生
        AudioManager.Ins.PlayOneShotSE(AudioManager.Ins.shootSEIndex);
            
        _canAttack=false;//攻撃できないようにする
        StartCoroutine(AttackCD());//一定時間後攻撃できるようにする
    }
    
    /// <summary>
    /// 移動
    /// </summary>
    void Move(){
        Vector2 tiltInput=GetJoyconTiltInput();//傾きの入力
        //Joy-Conのスティックのy
        float verticalStickInput=JoyConInputManager.Ins.HorizontalGripStickInput.y;
        
        if(tiltInput.magnitude<_deadZone){
            _moveVector
                =new Vector3(0,0,verticalStickInput*_stickMoveSpeed);
        }
        else{
            _moveVector
                =new Vector3(
                    tiltInput.x*_tiltMoveSpeed,
                    tiltInput.y*_tiltMoveSpeed,
                    verticalStickInput*_stickMoveSpeed
                );
        }

        //移動
        _rb.linearVelocity=_moveVector;
        
        //移動範囲以内に収める
        _pos=transform.position;
        _pos.x=Mathf.Clamp(_pos.x,_minMovementRangeX,_maxMovementRangeX);
        _pos.y=Mathf.Clamp(_pos.y,_minMovementRangeY,_maxMovementRangeY);
        _pos.z=Mathf.Clamp(_pos.z,_minMovementRangeZ,_maxMovementRangeZ);
        transform.position=_pos;
        
        Tilt(-tiltInput.y,-tiltInput.x);
    }

    /// <summary>
    /// プレイヤを傾ける
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    void Tilt(float x,float z){
        gameObject.transform.rotation=Quaternion.Euler(x*_tiltMultiplier,0,z*_tiltMultiplier);
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    void Attack(){
        //攻撃ボタンが押されているかつ、攻撃可能なとき攻撃
        if (JoyConInputManager.Ins.isAttackButton&&_canAttack){
            ShootBeam();
        }
    }
    
    //一定時間後攻撃できるようにする
    IEnumerator AttackCD(){
        yield return new WaitForSeconds(_attackInterval);
        _canAttack=true;
    }

    /// <summary>
    /// Joy-Conの傾きの入力と計算
    /// </summary>
    /// <returns>Playerの移動方向(x,y)</returns>
    Vector2 GetJoyconTiltInput(){
        Vector3 forward=JoyConInputManager.Ins.orientationDelta*Vector3.forward;
        Vector3 right=JoyConInputManager.Ins.orientationDelta*Vector3.right;

        float h,v;
        // Joy-Con(L)と(R)で傾きの向きが逆なので処理を分ける
        if(JoyConInputManager.Ins.joycon.isLeft){
            v=right.y;  // 上下
            h=forward.y;// 左右
        }
        else{
            v=-right.y;     //上下
            h=-forward.y;   //左右
        }

        return new Vector2(h,v);
    }

#if UNITY_EDITOR
    void OnDrawGizmos(){
        EditorGizmos.DrawWireCube(
            new Vector3(_minMovementRangeX,_minMovementRangeY,_minMovementRangeZ),
            new Vector3(_maxMovementRangeX,_maxMovementRangeY,_maxMovementRangeZ)
        );
    }
    
    /// <summary>
    /// デバッグ用の移動
    /// </summary>
    void DebugMove(){
        Debugger.Log("DebugMove");
        //移動方向の決定
        float v=Input.GetAxisRaw("Vertical");
        float h=Input.GetAxisRaw("Horizontal");
        
        _moveVector=new Vector3(h,v,0);

        //移動
        _rb.linearVelocity = _moveVector * _tiltMoveSpeed;
        
        _pos=transform.position;
        _pos.x=Mathf.Clamp(_pos.x,_minMovementRangeX,_maxMovementRangeX);
        _pos.y=Mathf.Clamp(_pos.y,_minMovementRangeY,_maxMovementRangeY);
        _pos.z=Mathf.Clamp(_pos.z,_minMovementRangeZ,_maxMovementRangeZ);
    }

    /// <summary>
    /// デバッグ用の攻撃
    /// </summary>
    void DebugAttack(){
        //Zが押されているかつ、攻撃可能なとき攻撃
        if (Input.GetKey(KeyCode.Z)&&_canAttack){
            ShootBeam();
        }
    }
#endif

    void OnTriggerEnter(Collider other){
        //当たったオブジェクトのビームスクリプトを取得
        Beam beam=other.gameObject.GetComponent<Beam>();
        if(!beam) return;
        //ビームが既に使われているなら処理しない(多重当たり防止)
        if(!beam.TryUse()) return;

        ApplyDamage(beam.attack);
        Destroy(beam.gameObject);
    }
    
    ///<summary>
    ///ダメージ処理
    ///</summary>
    void ApplyDamage(int damage){
        _hp-=damage;
        if(_hp>0) PlayHItEffect();
        else Die();
    }

    ///<summary>
    ///被弾時の演出
    ///</summary>
    void PlayHItEffect(){
        AudioManager.Ins.PlayOneShotSE(AudioManager.Ins.hitSEIndex);
    }

    /// <summary>
    /// 破壊処理
    /// </summary>
    void Die(){
        AudioManager.Ins.PlayOneShotSE(AudioManager.Ins.explosionSEIndex);
        Destroy(gameObject);
    }
}