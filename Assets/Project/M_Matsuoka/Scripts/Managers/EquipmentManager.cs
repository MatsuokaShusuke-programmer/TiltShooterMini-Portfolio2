using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 素材・武器・コストを管理するManager
/// </summary>
public class EquipmentManager:PersistentSingleton<EquipmentManager> {
    /*インベントリ:アイテムデータ(キー)と所次数(値)のペア
     * 設計書の"Inventory(Dictionary<ItemData,int>)"に対応*/
    public Dictionary<ItemData,int> Inventory
        =new Dictionary<ItemData,int>();

    //磯村=========================================================================
    [SerializeField] private int maxCost = 10;  //持ち込める武器の最大合計コスト
    public List<WeaponInfo> weapons = new List<WeaponInfo>();
    private int cost = 0;
    private int removeWeaponCost;
    //============================================================================

    /// <summary>
    /// 素材をインベントリに追加
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    public void AddMaterial(ItemData item,int amount = 1) {
        //引数のチェック
        if(!item) return;
        if (item.itemType != ItemType.Material) return;

        //既に素材を取ってるなら数を増やす
        if(Inventory.ContainsKey(item))
            Inventory[item]+=amount;
        //始めて入手する素材なら新しく登録
        else Inventory.Add(item,amount);
    }

    //磯村==========================================================
    /// <summary>
    /// 武器の追加とコスト加算
    /// </summary>
    /// <param name="weapon"></param>
    public void AddWeapon(WeaponInfo weapon)
    {
        //引数チェック
        if(weapon == null) return;

        //コストが最大コストを超える場合はreturn
        if (maxCost < cost + weapon.equipmentCost) return;

        //武器を追加し、現在のコストを増やす
        weapons.Add(weapon);
        cost += weapon.equipmentCost;
    }

    /// <summary>
    /// 武器の削除とコスト減算
    /// </summary>
    /// <param name="weaponNum"></param>
    public void RemoveWeapon(int weaponNum)
    {
        //引数チェック
        if (weapons[weaponNum] == null) return;

        //指定された番号の武器コスト保持
        removeWeaponCost = weapons[weaponNum].equipmentCost;

        //武器を減らし、現在のコストも減らす
        weapons.RemoveAt(weaponNum);
        cost -= removeWeaponCost;
    }
    //==============================================================
}
