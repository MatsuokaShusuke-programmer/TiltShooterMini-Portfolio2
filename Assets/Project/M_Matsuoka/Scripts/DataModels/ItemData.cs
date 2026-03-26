using UnityEngine;

/// <summary>
/// アイテムの種類
/// </summary>

public enum ItemType {
    Material,   //素材
    Heal,       //HP回復
    Fuel,       //燃料回復
    Oxygen      //酸素回復
}

/// <summary>
/// アイテム個別のデータを定義する
/// </summary>
[CreateAssetMenu(fileName = "NewItem",menuName = "Data/ItemData")]
public class ItemData:ScriptableObject {
    [Header("基本情報")]
    [Tooltip("アイテム名")]
    public string itemName;

    [Tooltip("アイテム説明")]
    [TextArea(3,5)]//Inspectorで複数行入力できるようにする
    public string description;

    [Tooltip("アイコン画像")]
    public Sprite icon;

    [Header("設定")]
    [Tooltip("アイテムの種類")]
    public ItemType itemType;

    [Tooltip("回復アイテムの場合の回復量(素材の場合は0でOK")]
    public float effectValue;
}
