using UnityEngine;

public class Earth:MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Enemy") || other.CompareTag("EnemyBullet")) {
            GameManager.OnEarthAttacked?.Invoke();

            if (GameManager.OnEarthAttacked == null)
            {
                Debug.LogError("ゲームマネージャーに被弾処理が登録されていません");
            }
            //else
            //{
            //    Debug.Log("被弾処理ちゃんとあるよ～");
            //}

            other.gameObject.SetActive(false);
        }
    }
}
