using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/スキルデータ")]
public class SkillData : ScriptableObject
{
    public SkillInfo[] skills;
}

[System.Serializable]
public class SkillInfo
{
    public string skillName;
    [TextArea]
    public string description;
    public float duration;
    public int healAmount;


    public GameObject prefab;
}