using UnityEngine;

public class ItemInformation : MonoBehaviour
{
    [Header("アイテムの種類")]
    public ItemData data;

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float deleteTime = 5.0f;

    //変数
    private Transform target;
    private Transform myTransform;
    private Vector3 dir;
    private void Start()
    {
        //プレイヤーの方向取得
        target = GameObject.FindWithTag("Earth").transform;
        myTransform = transform;
        dir = (target.position - myTransform.position).normalized;

        //自身を生成されてから一定時間後に消去
        Destroy(gameObject, deleteTime);
    }

    private void Update()
    {
        //プレイヤーの方向に飛んでいく
        myTransform.position += dir * Time.deltaTime * speed;
    }
}
