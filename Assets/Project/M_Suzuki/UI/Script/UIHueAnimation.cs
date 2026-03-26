using System.Collections;
using UnityEngine;

public class UIHueAnimation : MonoBehaviour
{
    public RectTransform target;//アニメーションの対象
    public Vector2 addStartPos;//アニメーションの相対的開始位置
    //public bool playOnStart = true;//出現と同時に再生するか?

    public float speed = 1.5f;//アニメーションスピード
    public float playTeasing = 0;//開始タイミングのズレ

    Vector2 startPos;

    private void Awake()
    {
        startPos = target.anchoredPosition;
        // if (playOnStart) HuedinAnimation();
    }

    public Coroutine HuedinAnimation()
    {
        return StartCoroutine(Animation(true));
    }

    public Coroutine HuedoutAnimation()
    {
        return StartCoroutine(Animation(false));
    }

    IEnumerator Animation(bool hue)
    {
        target.anchoredPosition = startPos + addStartPos;

        yield return new WaitForSeconds(playTeasing);

        for (float t = 0; t < 1; t+= Time.deltaTime * speed)
        {
            if(hue)
            {
                target.anchoredPosition = UIMove.Huddoin(startPos + addStartPos, startPos, t);
            }
            else
            {
                target.anchoredPosition = UIMove.Huddoout(startPos, startPos + addStartPos, t);
            }

            yield return null;
        }

        if (hue)
        {
            target.anchoredPosition = startPos;
        }
        else
        {
            target.anchoredPosition = startPos + addStartPos;
        }
    }
}
