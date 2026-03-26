using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultDisplay : MonoBehaviour
{
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject lossUI;

    [SerializeField] UIHueAnimation okUI;
    GameObject okUIObj;

    [SerializeField] TMP_Text reportingText;


    private void Start()
    {
        reportingText.text = "";

        lossUI.SetActive(false);
        winUI.SetActive(false);

        okUIObj = okUI.gameObject;
        okUIObj.SetActive(false);
    }

    public void ResultRequest(bool win , int xp , Dictionary<ItemData, int> getMaterial , Action callback = null)
    {
        StartCoroutine(Result(win, xp, getMaterial, callback));
    }

    IEnumerator Result(bool win, int xp, Dictionary<ItemData, int> getMaterial, Action callback)
    {
        reportingText.text = $"「経験値」\n{xp.ToString()}\n\n";

        string getMaterialText = "「取得素材」\n";

        foreach (var item in getMaterial)
        {
            getMaterialText += $"{item.Key.itemName}_{item.Value}個\n";
        }

        reportingText.text += getMaterialText;

        if (win)
        {
            winUI.SetActive(true);
            lossUI.SetActive(false);
        }
        else
        {
            winUI.SetActive(false);
            lossUI.SetActive(true);
        }

        okUIObj.SetActive(true);
        okUI.HuedinAnimation();

        yield return null;

        if (callback != null) callback();
    }

    public void NextRequest(string sceneName)
    {
        GameManager.Instance.isPlaying = true;
        GameManager.Instance.StateReset();

        GameManager.Instance.ChangeScene(sceneName);
    }
}
