using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/武具データ")]
public class WeaponData : ScriptableObject
{
    public WeaponInfo[] weapons;
}

[System.Serializable]
public class WeaponInfo
{
    public string weaponName;
    public WeaponType weaponType;
    [TextArea]
    public string description;
    // craftCost;
    public int equipmentCost;

    public GameObject prefab;

    public bool isEquip;

    public enum WeaponType
    {
        Standard,//標準装備
        Option,//アップグレード
    }
}


//WeaponName(string): 武具名（例: "アースバリア"） 。
//WeaponType(enum): 初期装備 / 制作可能 。
//Description(string): 機能説明 。
//CraftCost (List<ItemCost>): 制作・強化に必要な素材 。
//EquipmentCost (int): 装備コスト 。