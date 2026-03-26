using UnityEngine;

public class SkillBarrier : Skill
{
    private void Start()
    {
        DeadCount(Info.duration);
    }
}
