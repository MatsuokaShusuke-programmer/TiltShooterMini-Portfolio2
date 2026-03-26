using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public enum BulletType
{
    Circle,
    FirstTracking,
    Tracking
}

public enum EnemyType
{
    Normal,
    Circle,
    Zigzag
}

[System.Serializable]
public class BulletData
{
    public GameObject bullet;
    public float speed = 5.0f;
    public float lifeTime = 5.0f;
    public BulletType bulletType = BulletType.FirstTracking;
    public int circleAngleSpeed = 100;

    [HideInInspector] public Transform bulletTransform;
    [HideInInspector] public Vector3 direction;
    //キャッシュ
    [System.NonSerialized]public EnemyBullet enemyBullet;
}

[System.Serializable]
public class EnemyInfo
{
    public Transform targetTransform;         //ターゲットの情報はEnemyManagerから取得予定
    public Transform attackPos;
    public float speed = 3.0f;
    public float attackCoolTime = 1.0f;
    public float hp = 100;
    public float damage = 10;
    public bool isItemDrop = true;
    public ItemDropDatas dropItem;
    public EnemyType enemyType = EnemyType.Normal;
    public int angleSpeed = 1;
    public int angleRange = 20;
}
public class CreateBulletObjectPool : MonoBehaviour
{
    protected BulletData bulletDataTemp;
    protected List<BulletData> bullets = new List<BulletData>(); 
    protected float _deltaTime;
    protected float attackTime;
    protected float[] sin = new float[3600];
    protected float[] cos = new float[3600];

    private int dirCount = 0;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        //サインコサインの値を保持
        for (int i = 0; i < 3600; i++)
        {
            sin[i] = Mathf.Sin(i * 0.1f * Mathf.Deg2Rad);
            cos[i] = Mathf.Cos(i * 0.1f* Mathf.Deg2Rad);
        }
    }
    //複数個用意
    protected void CreateObjectPool(GameObject bullet,int max)
    {
        for(int i = 0; i < max; i++)
        {
            CreateObjectPool(bullet);
        }
    }

    protected void DeleteObjectPool()
    {
        bullets.Clear();
    }

    //一個用意
    protected void CreateObjectPool(GameObject bullet)
    {
        bulletDataTemp = new BulletData();
        bulletDataTemp.bullet = Instantiate(bullet,transform);
        bulletDataTemp.bullet.SetActive(false);
        bulletDataTemp.bulletTransform = bulletDataTemp.bullet.transform;

        bulletDataTemp.enemyBullet
            =bulletDataTemp.bullet.GetComponent<EnemyBullet>();
        //弾にEnemyBulletが無ければ自動でつける
        if(!bulletDataTemp.enemyBullet)
            bulletDataTemp.enemyBullet
                =bulletDataTemp.bullet.AddComponent<EnemyBullet>();

        bullets.Add(bulletDataTemp);
    }

    //弾を発射する
    protected void Attack(EnemyInfo bulletInfo,BulletData bulletData,int circleCountUpDir = 10)
    {
        if (attackTime < bulletInfo.attackCoolTime) return;
        AudioManager.Instance.PlaySE(2);

        //使われていない弾を参照
        for (int i = 0; i < bullets.Count; i++)
        {
            //すべての弾が使われている場合
            if (i == bullets.Count - 1 && bullets[i].bullet.activeInHierarchy)
            {
                CreateObjectPool(bulletData.bullet);
                i++;
            }

            //使われている弾の場合
            if (bullets[i].bullet.activeInHierarchy) continue;

            bullets[i].bullet.SetActive(true);
            bullets[i].bulletTransform.position = bulletInfo.attackPos.position;

            //ダメージ値を弾に渡す
            if(bullets[i].enemyBullet)
                bullets[i].enemyBullet.damage=bulletInfo.damage;

            switch (bulletData.bulletType)
            {
                //最初だけターゲットの方向を取得
                case BulletType.FirstTracking:
                    bullets[i].direction = (bulletInfo.targetTransform.position - bulletInfo.attackPos.position).normalized;
                    break;

                //円形状に弾を発射
                case BulletType.Circle:
                    bullets[i].direction.x = sin[dirCount];
                    bullets[i].direction.z = cos[dirCount];
                    dirCount = AdjustAngle(dirCount + circleCountUpDir);
                    //if (dirCount + circleCountUpDir > 3600)
                    //{
                    //    dirCount += circleCountUpDir;
                    //    dirCount -= 3600;
                    //}
                    //else dirCount += circleCountUpDir;
                    break;
            }

            attackTime = 0;

            break;
        }
    }

    //発射した弾の移動を制御
    protected void Bullet(EnemyInfo enemyData, BulletData bulletData,float defBulletLifeTime = 5.0f)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (!bullets[i].bullet.activeInHierarchy) continue;
            else
            {
                switch (bulletData.bulletType)
                {
                    //移動中は対象の方向を常に取得
                    case BulletType.Tracking:
                        bullets[i].direction = (enemyData.targetTransform.position - enemyData.attackPos.position).normalized;
                        break;
                    default:
                        break;
                }
                //使われている弾だけ動かす
                bullets[i].bulletTransform.position += bullets[i].direction * bulletData.speed * _deltaTime;

                //生存時間を過ぎた弾はプールに返還
                bullets[i].lifeTime -= _deltaTime;
                if (bullets[i].lifeTime <= 0)
                {
                    bullets[i].bullet.SetActive(false);
                    bullets[i].lifeTime = defBulletLifeTime;
                }
            }
        }
    }

    //２点間の角度を求める
    protected int GetAngle(Vector3 targetPos,Vector3 myPos)
    {
        //方向ベクトル取得
        Vector3 dt = myPos - targetPos;
        float rad = Mathf.Atan2(dt.z, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        return (int)degree;
    }

    //角度を０～３５９．９度に直す
    protected int AdjustAngle(int angle)
    {
        return (angle % 3600 + 3600) % 3600;
    }

}
