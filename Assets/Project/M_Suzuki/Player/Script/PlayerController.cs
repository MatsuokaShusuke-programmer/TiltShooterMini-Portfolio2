using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    const float ANGLE_MISALIGNMENT = 90;//開始角度のズレ

    [SerializeField] Transform playerTransform;
    [SerializeField] float speed;

    [SerializeField] float dashSize;
    [SerializeField] float dashTime;

    float t = 0;
    float height;

    float inputAxis;
    bool isDash = false;

    float rot = 0;
    private void OnMove(InputValue value)
    {
        Vector2 axis= value.Get<Vector2>();

        inputAxis = Mathf.Clamp(-axis.x + axis.y , -1 ,1);
    }


    private void OnBoost()
    {
        if(!isDash)
        {
            isDash = true;

            t = dashTime;
        }
    }

    private void Start()
    {
        height = Vector3.Distance(transform.position, playerTransform.position);//高さを記憶
    }


    private void Update()
    {
        if (!GameManager.Instance.isPlaying) return;
        Move();
        DashTime();
    }

    void Move()//移動
    {

        float amount = (inputAxis * speed * Time.deltaTime) / height;//移動量を算出

        if(isDash) amount *= dashSize;//ダッシュ倍率を賭ける

        rot += amount;//角度を反映

        //ResurceManagerが存在してるかつ、移動してるとき
        if(ResourceManager.Instance&&amount!=0) {
            //絶対値にして渡す
            float consumption = Mathf.Abs(amount);

            if (isDash)
            {
                ResourceManager.Instance.OnPlayerBoost(consumption);
            }
            else
            {
                ResourceManager.Instance.OnPlayerMove(consumption);
            }
        }

        //ポジションに変換
        Vector3 pos = new Vector3(Mathf.Cos((rot + ANGLE_MISALIGNMENT) * Mathf.Deg2Rad), playerTransform.position.y, Mathf.Sin((rot + ANGLE_MISALIGNMENT) * Mathf.Deg2Rad));
        pos *= height;

        playerTransform.position = pos;//位置反映

        playerTransform.rotation = Quaternion.Euler(0,-rot,0);//回転反映
    }

    void DashTime()//ダッシュ管理
    {
        if(t <= 0)
        {
            isDash = false;
        }
        else
        {
            t -= Time.deltaTime;
        }
    }
}
