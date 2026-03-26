using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            float damage = 0;
            bool isHit = false;

            if (other.CompareTag("EnemyBullet"))
            {
                isHit = true;

                //弾のスクリプトからダメージを取得
                EnemyBullet eb = other.GetComponent<EnemyBullet>();
                if (eb)
                    damage = eb.damage;
                else
                    damage = 1f;
            }
            else if (other.CompareTag("Enemy"))
            {
                isHit = true;

                //敵のスクリプトからダメージを取得
                EnemyController ec = other.GetComponent<EnemyController>();
                if (ec)
                    damage = ec.Damage;
                else
                    damage = 1f;
            }

            if (isHit)
            {
                if (ResourceManager.Instance)
                    ResourceManager.Instance.OnPlayerDamage(damage);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.CompareTag("Item"))
        {
            ItemData item = other.GetComponent<ItemInformation>().data;
            EquipmentManager.Ins.AddMaterial(item);
            ResourceManager.Instance.OnCollectItem(item);
            Destroy(other.gameObject);
        }
    }
}
