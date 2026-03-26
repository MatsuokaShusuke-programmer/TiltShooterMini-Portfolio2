using System.Collections.Generic;
using UnityEngine;

public class ResultTest : MonoBehaviour
{
    [SerializeField] ResultDisplay resultDisplay;

    [SerializeField] bool win;

    [SerializeField] int xp;

    [SerializeField] List<DebugGetMaterial> getMaterial; //Debug用習得素材リスト

    [ContextMenu("リザルトテスト")]
    void Result()
    {
        Dictionary<ItemData, int> material = new Dictionary<ItemData, int>();//Dictionaryに変換
        foreach (var m in getMaterial)
        {
            if (m.item == null) continue;

            material[m.item] = m.count;
        }

        resultDisplay.ResultRequest(win , xp , material, ResultEnd);
    }

    void ResultEnd()
    {
        Debug.Log("リザルト終了");
    }

    [System.Serializable]
    public class DebugGetMaterial//Debug用習得素材リスト用
    {
        public ItemData item;
        public int count;
    }
}
