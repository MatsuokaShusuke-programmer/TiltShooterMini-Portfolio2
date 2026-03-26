using System;
using UnityEngine;

[System.Serializable]
public class ResourceOption
{

    [Header("最大値")]
    public float maxHP = 1;
    public float maxFuel = 1;
    public float maxOxygen = 1;
    [Header("酸素の減少スピード")]
    public float oxygenDecreaseSpeed = 0.01f;
}
public class ResourceManager : MonoBehaviour
{
    [SerializeField] private ResourceOption resourceOption;
 
    public static ResourceManager Instance { get; private set; }
    public float currentHP {  get; private set; }       //耐久値
    public float currentFuel { get; private set; }      //燃料
    public float currentOxygen { get; private set; }    //酸素

    //リソース不足イベント
    public event Action OnHPDepleted;
    public event Action OnFuelDepleted;
    public event Action OnOxygenDepleted;

    //変数
    private float _deltaTime;
    private ParameterUI _parameterUI;
    private float correctionValue = 0.001f;
    private void Awake()
    {
        //シングルトン化
        if(Instance != null) Destroy(gameObject);
        else Instance = this;

        //初期値設定
        currentHP = resourceOption.maxHP;
        currentFuel = resourceOption.maxFuel;
        currentOxygen = resourceOption.maxOxygen;

        _parameterUI = GetComponent<ParameterUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _deltaTime = Time.deltaTime;

        if (!GameManager.Instance.isPlaying) return;    //ゲームプレイ中以外は反映されない
        //酸素の減少
        if(currentOxygen > 0) {
            currentOxygen -= _deltaTime * resourceOption.oxygenDecreaseSpeed;

            if(currentOxygen <= 0) {
                currentOxygen = 0;
                OnOxygenDepleted?.Invoke();//酸欠通知
            }
        }

        _parameterUI.SetFuel(currentFuel);
        _parameterUI.SetHP(currentHP);
        _parameterUI.SetOTwo(currentOxygen);
    }

    public void OnPlayerMove(float moveAmount)
    {
        currentFuel -= moveAmount * correctionValue;
        CheckFuel();
    }

    public void OnPlayerBoost(float boostAmount)
    {
        currentFuel -= boostAmount * correctionValue;
        CheckFuel();
    }

    /// <summary>
    /// 燃料の確認
    /// </summary>
    public void CheckFuel() {
        if(currentFuel<=0) {
            currentFuel=0;
            OnFuelDepleted?.Invoke();//燃料不足通知
        }
    }

    public void OnPlayerDamage(float damage)
    {
        currentHP -= damage * correctionValue;
        if(currentHP<=0) {
            currentHP=0;
            OnHPDepleted?.Invoke();//HP0通知
        }
    }

    public void OnCollectItem(ItemData item)
    {
        switch (item.itemType)
        {
            //回復
            case ItemType.Heal:
                currentHP += item.effectValue;
                //HPがMaxHPよりおおきいときMaxHPにする
                currentHP=currentHP>resourceOption.maxHP ? resourceOption.maxHP : currentHP;
                break;

            //燃料
            case ItemType.Fuel:
                currentFuel += item.effectValue;
                break;

            //酸素
            case ItemType.Oxygen:
                currentOxygen += item.effectValue;
                break;

            //素材
            case ItemType.Material:
                EquipmentManager.Ins.AddMaterial(item);
                break;

        }

        
    }

    public void OnUseSkill(SkillData skill)
    {
        //ここにスキル処理を記入
    }
}
