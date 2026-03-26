using UnityEngine;

/// <summary>
/// ’n‹…‚ÌŠÇ—
/// </summary>
public class EarthController:MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        // “GA‚Ü‚½‚Í“G‚Ì’e‚É“–‚½‚Á‚½‚Æ‚«
        if(other.CompareTag("Enemy")||other.CompareTag("EnemyBullet")) {
            // GameManager‚É’Ê’m
            GameManager.OnEarthAttacked?.Invoke();

            // “–‚½‚Á‚½‚à‚Ì‚ğÁ‚·
            Destroy(other.gameObject);
        }
    }
}