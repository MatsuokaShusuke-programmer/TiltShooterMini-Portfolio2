using UnityEngine;

/// <summary>
/// 敵の弾にアタッチし、ダメージ値を保持するクラス
/// </summary>
public class EnemyBullet:MonoBehaviour {
    // ダメージ値を保持（EnemyControllerから発射時に代入される）
    [System.NonSerialized] public float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet")) gameObject.SetActive(false);
    }
}