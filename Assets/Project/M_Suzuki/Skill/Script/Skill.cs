using System.Collections;
using UnityEngine;

public class Skill : MonoBehaviour
{
    SkillInfo skillInfo;

    Coroutine deadCoroutine;

    public SkillInfo Info
    {
        get { return skillInfo; }
        set { skillInfo = value; }
    }

    protected virtual void DeadCount(float t)
    {
        if(deadCoroutine == null) deadCoroutine = StartCoroutine(Dead(t));
    }

    IEnumerator Dead(float t)
    {
        yield return new WaitForSeconds(t);

        Destroy(gameObject);
    }
}
