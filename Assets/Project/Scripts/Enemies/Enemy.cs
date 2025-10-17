using UnityEngine;

public class Enemy:MonoBehaviour{
    [Header("ステータス")]
    [SerializeField]int _hp=1;
    [SerializeField]int _attack=1;
    [SerializeField]float _attackCD=1f;//攻撃の間隔(s)
    [SerializeField]int _score=1;//破壊時に加算されるスコア

    [SerializeField]GameObject _beam;//ビーム
    [SerializeField]float _addPos=1f;//ビームをどれだけ前に出すか

    [SerializeField]float _maxDistance=50f;//レイを飛ばす距離
    
    float _checkRadius=1f;//レイの半径

    Vector3 _beamPos;//ビームの生成位置

    bool _isDead=false;//破壊されたか
    bool _canAttack=true;//攻撃可能か

    Quaternion _beamRot;//ビームの角度

    void Start(){
        //ビームのスポーン場所を敵の前にする
        _beamPos=transform.position;
        _beamPos.z-=_addPos;
        _beamRot=transform.rotation;//ビームの角度を敵とおなじにする

        //コライダの太さの半径を取得
        _checkRadius=_beam.GetComponent<Collider>().bounds.extents.x;
    }

    void Update(){
        if(_canAttack)Attack();//攻撃可能なら攻撃
    }

    void OnTriggerEnter(Collider other){
        if(_isDead) return;//既にはかいされたならリターン

        //当たったオブジェクトのビームスクリプトを取得
        Beam beam=other.gameObject.GetComponent<Beam>();
        if(!beam) return;
        //ビームが既に使われているなら処理しない(多重当たり防止)
        if(!beam.TryUse()) return;

        ApplyDamage(beam.attack);
        Destroy(beam.gameObject);
    }

    //攻撃
    void Attack(){
        //前にEnemyがいるなら撃たない
        if(IsAnotherEnemyInFront()) return;

        //ビーム生成
        Instantiate(_beam,_beamPos,_beamRot);

        //発射音再生
        AudioManager.Ins.PlayOneShotSE(AudioManager.Ins.shootSEIndex);
        
        _canAttack=false;//攻撃不可にする
        Invoke(nameof(ResetAttack), _attackCD);//一定時間後に攻撃可能にする
    }

    //攻撃可能にする
    void ResetAttack() => _canAttack = true;

    ///<summary>
    /// 自分の正面に Enemy がいるか（最初に当たった物体が Enemy か）を判定
    /// </summary>
    bool IsAnotherEnemyInFront(){
        // 自分のコライダーより少し前から判定を始め、自己ヒットを避ける
        var myCol=GetComponent<Collider>();
        Vector3 origin=(myCol!=null)
            ?myCol.bounds.center+transform.forward*(myCol.bounds.extents.magnitude+0.01f)
            :transform.position+transform.forward*0.5f;

        // 前方に“Enemy”がいるかだけを見る（壁やプレイヤーは無視）
        var hits=Physics.SphereCastAll(
            origin,
            _checkRadius,
            transform.forward,
            _maxDistance,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (var h in hits){
            if(myCol!=null&&h.collider==myCol) continue;// 自分は無視
            if(h.collider.GetComponent<Enemy>()!=null) return true;// 別の Enemy が前にいる
        }

        return false;// 前に Enemy なし
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
        Debugger.Log("敵死亡");

        _isDead=true;

        AudioManager.Ins.PlayOneShotSE(AudioManager.Ins.explosionSEIndex);
        AddScore();
        
        EnemySpawner.Ins.currentEnemyNum--;//現在の敵の数を1減らす
        EnemySpawner.Ins.ReSpawn();
        
        Destroy(gameObject);
    }

    /// <summary>
    /// スコア加算
    /// </summary>
    void AddScore(){
        //GameManagerがnullのとき警告
        if(!GameManager.Ins){
            Debugger.LogWarning("GameManagerが見つかりません。");
            return;
        }

        //スコアマネージャの取得
        ScoreManager scoreManager=GameManager.Ins.scoreManager;
        //スコアマネージャがnullなら警告
        if(!scoreManager){
            Debugger.LogWarning("ScoreManagerがGameManagerに登録されていません。");
            return;
        }

        //スコアを加算
        scoreManager.AddScore(_score);
    }
}