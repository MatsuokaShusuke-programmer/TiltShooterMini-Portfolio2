using UnityEngine;

public class EnemyController : CreateBulletObjectPool
{
    [Header("Enemyの設定")]
    [SerializeField] private EnemyInfo enemyData;

    //外部からダメージを参照するための変数
    public float Damage => enemyData.damage;

    [Header("弾の設定")]
    [SerializeField] private BulletData bulletData;
    [SerializeField] private int objectPoorMaxIndex = 10;
    [SerializeField] private int circleCountUpDir = 10;

    //変数
    private Transform myTransform;

    private int startAngle;
    private int angle;
    private int countAngle;
    private bool isUpAngle = true;
    private float r;
    
    private Vector3 myDir;
    private float defBulletLifeTime;
    float _currentHP;//現在のHP
    private int itemNum;
   
    protected override void Start()
    {
        base.Start();

        //各種情報の保持
        myTransform = transform;
        enemyData.targetTransform = GameObject.FindWithTag("Earth").transform;

        r = Mathf.Sqrt(Mathf.Abs(myTransform.position.x - enemyData.targetTransform.position.x)
            * Mathf.Abs(myTransform.position.x - enemyData.targetTransform.position.x)
            + Mathf.Abs(myTransform.position.z - enemyData.targetTransform.position.z)
            * Mathf.Abs(myTransform.position.z - enemyData.targetTransform.position.z));

        startAngle = GetAngle(enemyData.targetTransform.position, myTransform.position);
        angle = startAngle;
        

        defBulletLifeTime = bulletData.lifeTime;

        //HPのコピー
        _currentHP = enemyData.hp;

        if(enemyData.targetTransform)
            myDir
                =(enemyData.targetTransform.position-myTransform.position).
                normalized;

        //オブジェクトプール化
        CreateObjectPool(bulletData.bullet, objectPoorMaxIndex);
    }

    void Update()
    {
        //ターゲットがいなければ動作しない
        if(!enemyData.targetTransform) return;

        if (!GameManager.Instance.isPlaying)
        {
            DeleteObjectPool();
            Destroy(gameObject);
        }

        _deltaTime = Time.deltaTime;
        attackTime += _deltaTime;

        EnemyMove();

        Attack(enemyData,bulletData,bulletData.circleAngleSpeed);
        
        //弾の移動と生存時間
        Bullet(enemyData,bulletData,defBulletLifeTime);

    }

    private void EnemyMove()
    {
        switch (enemyData.enemyType)
        {
            //毎フレーム敵の移動の向きを更新することで追尾できるようにする
            case EnemyType.Normal:
                myDir = (enemyData.targetTransform.position - myTransform.position).normalized;
                //自身の移動
                myTransform.position += myDir * enemyData.speed * _deltaTime;
                break;

            //ターゲットの周りをぐるぐる
            case EnemyType.Circle:
                //接触した場合
                if (r <= 0) return;

                r -= enemyData.speed * _deltaTime;

                //角度の範囲は０～３５９９まで(０から３５９．９度)
                angle = AdjustAngle(angle + enemyData.angleSpeed);

                myDir.x = enemyData.targetTransform.position.x + cos[angle] * r;
                myDir.z = enemyData.targetTransform.position.z + sin[angle] * r;
                myTransform.position = myDir;
                break;

            //ジグザグ移動
            case EnemyType.Zigzag:
                //接触した場合
                if (r <= 0) return;

                r -= enemyData.speed * _deltaTime;

                if (countAngle >= enemyData.angleRange)
                {
                    countAngle -= enemyData.angleRange;
                    isUpAngle = !isUpAngle;
                    if (isUpAngle) angle = AdjustAngle(angle + enemyData.angleSpeed);
                    else angle = AdjustAngle(angle - enemyData.angleSpeed);
                }
                else
                {
                    if (isUpAngle) angle = AdjustAngle(angle + enemyData.angleSpeed);
                    else angle = AdjustAngle(angle - enemyData.angleSpeed);
                    countAngle++;
                }

                myDir.x = enemyData.targetTransform.position.x + cos[angle] * r;
                myDir.z = enemyData.targetTransform.position.z + sin[angle] * r;
                myTransform.position = myDir;
                break;
        }

        //どの攻撃方法もターゲットを向く
        myTransform.LookAt(enemyData.targetTransform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            enemyData.hp -= enemyData.damage;
            AudioManager.Instance.PlaySE(0);

            if (enemyData.hp > 0) return;

            //ドロップあり
            if (enemyData.isItemDrop)
            {
                itemNum = Random.Range(0, enemyData.dropItem.data.Length - 1);
                //ドロップアイテムを生成し、自身は消える(オブジェクトプールを使う場合ここを書き換え)
                GameObject dropItem = Instantiate(enemyData.dropItem.data[itemNum].itemObj);
                dropItem.transform.position = myTransform.position;
            }
            Destroy(gameObject);
        }
    }
}
