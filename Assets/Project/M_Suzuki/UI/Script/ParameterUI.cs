using UnityEngine;
using UnityEngine.UI;

public class ParameterUI : MonoBehaviour
{
    [SerializeField] Image hp;
    [SerializeField] Image oTwo;
    [SerializeField] Image fuel;

    [SerializeField] float changeSpeed;

    float hpSize   = 1;//0～1
    float oTwoSize = 1;//0～1
    float fuelSize = 1;//0～1

    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if(!gameManager.isPlaying) gameObject.SetActive(false);

        hp.fillAmount = Mathf.Lerp(hp.fillAmount, hpSize, changeSpeed * Time.deltaTime);
        oTwo.fillAmount = Mathf.Lerp(oTwo.fillAmount, oTwoSize, changeSpeed * Time.deltaTime);
        fuel.fillAmount = Mathf.Lerp(fuel.fillAmount, fuelSize, changeSpeed * Time.deltaTime);

    }

    public void SetHP(float value)
    {
        ValueChange(ref hpSize , value);
    }

    public void SetOTwo(float value)
    {
        ValueChange(ref oTwoSize, value);
    }

    public void SetFuel(float value)
    {
        ValueChange(ref fuelSize, value);
    }

    void ValueChange(ref float variable , float value)
    {
        if(value > 1) value = 1;//範囲を超えていたら最大値にする
        if(value < 0) value = 0;//範囲を下回っていたら最小値にする

        variable = value;
    }
}
