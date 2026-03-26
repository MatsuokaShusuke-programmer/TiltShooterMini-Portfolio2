using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] Transform armed;
    [SerializeField] float coolTime;
    [SerializeField] WeaponData weaponData;
    Weapon[] weapons;
    float t;

    private void Start()
    {
        SetUp();
    }

    private void Update()
    {
        if(t > coolTime)
        {
            Fire();
            t = 0;
        }
        else
        {
            t += Time.deltaTime;
        }
    }

    void SetUp()
    {

        List<GameObject> memory = new List<GameObject>();
        for (int i = 0; i < weaponData.weapons.Length; i++)
        {
            if (weaponData.weapons[i].isEquip)
            {
                memory.Add(Instantiate(weaponData.weapons[i].prefab , armed));
            }

        }

        weapons = new Weapon[memory.Count];

        for (int i = 0; i < memory.Count; i++)
        {
            weapons[i] = memory[i].GetComponent<Weapon>();
        }

        t = coolTime;
    }

    void Fire()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Fire();
        }

    }
}
