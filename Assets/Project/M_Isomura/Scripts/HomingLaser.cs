using UnityEngine;

public class HomingLaser : CreateBulletObjectPool
{
    [Header("発射元の情報")]
    [SerializeField] private EnemyInfo homingData;

    [Header("弾の情報")]
    [SerializeField] private BulletData bulletData;
    [SerializeField] private int objectPoorMaxIndex = 10;
    [SerializeField] private int circleCountUpDir = 10;

    private Transform myTransform;
    private float defBulletLifeTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        //トランスフォームを保持
        myTransform = transform;

        defBulletLifeTime = bulletData.lifeTime;

        //オブジェクトプールを作成
        CreateObjectPool(bulletData.bullet, objectPoorMaxIndex);
    }

    // Update is called once per frame
    void Update()
    {
        _deltaTime = Time.deltaTime;
        //ターゲットがいない場合は起動しない
        if (homingData.targetTransform == null) return;
        attackTime += _deltaTime;

        //攻撃時の処理
        Attack(homingData,bulletData);

        //弾の移動と生存時間
        Bullet(homingData,bulletData,defBulletLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //ターゲットを指定
            homingData.targetTransform = other.transform;
        }
    }
}
