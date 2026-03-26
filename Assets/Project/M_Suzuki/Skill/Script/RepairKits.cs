using UnityEngine;

public class RepairKits : Skill
{
    private void Start()
    {
        Healing(Info.healAmount);

        Destroy(gameObject);
    }


    void Healing(int addHp/*回復量*/)
    {
        //自機回復処理
    }
}
