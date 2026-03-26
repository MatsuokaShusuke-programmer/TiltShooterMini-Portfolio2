using UnityEngine;

public enum ItemState
{
    //ここにアイテムの種類、もしくは素材の種類を記入する
    Fuel,
    Oxygen,
    Heal,
    Material
}

[CreateAssetMenu(fileName = "ItemDropData", menuName = "Scriptable Objects/ItemDropData")]
public class ItemDropData : ScriptableObject
{
    [Header("アイテムと素材に関するデータ")]
    public GameObject itemObj;
    public ItemState itemState;
    public float value;
}
