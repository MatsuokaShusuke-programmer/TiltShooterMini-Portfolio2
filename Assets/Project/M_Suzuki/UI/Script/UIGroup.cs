using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGroup : MonoBehaviour
{
    [SerializeField] bool isOpen;

    [SerializeField] GameObject[] _UIobj;

    UI[] uI;

    public struct UI
    {
        public GameObject obj;

        public UIHueAnimation animation;
    }

    private void Start()
    {
        uI = new UI[_UIobj.Length];

        for (int i = 0; i < _UIobj.Length; i++)
        {
            uI[i].obj = _UIobj[i];

            if (_UIobj[i].TryGetComponent<UIHueAnimation>(out UIHueAnimation uiAnimation))
            {
                uI[i].animation = uiAnimation;
            }
        }

        if (isOpen) OpenUI();
    }

    /// <summary>
    /// UIを表示し、出現アニメを再生
    /// </summary>
    public void OpenUI()
    {
        AudioManager.Instance.PlaySE(3);
        for (int i = 0; i < uI.Length; i++)
        {
            uI[i].obj.SetActive(true);

            if (uI[i].animation != null) uI[i].animation.HuedinAnimation();
        }

        isOpen = true;
    }

    public void ClauseUI()
    {
        StartCoroutine(ClauseUiCoroutine());
    }

    IEnumerator ClauseUiCoroutine()
    {
        List<Coroutine> waitCoroutines = new List<Coroutine>();

        for (int i = 0; i < uI.Length; i++)
        {
            if (uI[i].animation != null)  waitCoroutines.Add(uI[i].animation.HuedoutAnimation());
        }

        for (int i = 0; i < waitCoroutines.Count; i++)
        {
            yield return waitCoroutines[i];
        }

        for (int i = 0; i < uI.Length; i++)
        {
            uI[i].obj.SetActive(false);
        }

        isOpen = false;
    }
}
