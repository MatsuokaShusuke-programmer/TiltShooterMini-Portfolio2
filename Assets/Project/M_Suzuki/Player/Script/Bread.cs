using UnityEngine;

public class Bread : MonoBehaviour
{
    public Vector3 vector;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.position += vector * Time.deltaTime;

        if (!spriteRenderer.isVisible)
        {
            Destroy(gameObject);
        }
    }
}
