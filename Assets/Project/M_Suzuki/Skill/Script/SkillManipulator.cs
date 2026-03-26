using UnityEngine;

public class SkillManipulator : MonoBehaviour
{

    /*Debug用スキル設定ができるまで用*/

    [SerializeField] SkillData skillData;

    [SerializeField] int skillNum;


    void SetSkill()
    {
        skillInfo = skillData.skills[skillNum];
    }

    /*///////////////////////////////*/

    [SerializeField] Transform skillBasis;
    [SerializeField] float recastTime;

    float t;

    SkillInfo skillInfo;

    GameObject skillPrefab;

    public SkillInfo SkillInfo { get { return skillInfo; } }

    private void OnSkill()
    {
        BootSkill();
    }

    private void Start()
    {
        SetSkill();//※Debug用スキル設定ができるまで用

        SetUp();
    }

    private void Update()
    {
        if(t >= 0)
        {
            t -= Time.deltaTime;
        }
    }

    void SetUp()
    {
        skillPrefab = skillInfo.prefab;
    }

    void BootSkill()
    {
        if (t > 0) return;

        Skill skill = Instantiate(skillPrefab , skillBasis).GetComponent<Skill>();

        skill.Info = skillInfo;

        t = recastTime;
    }
}
