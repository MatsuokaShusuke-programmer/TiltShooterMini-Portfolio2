using UnityEngine;

public class Roll : MonoBehaviour
{
    [SerializeField] float speed; // 1秒あたりの回転量

    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
    }
}
