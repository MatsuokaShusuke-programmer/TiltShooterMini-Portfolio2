using System.Collections;
using UnityEngine;

public class Laser : Weapon
{
    [SerializeField] float speed;
    [SerializeField] int shotNum;
    [SerializeField] GameObject breadPrefab;

    [SerializeField] Transform[] shotPoss;

    public override void Fire()
    {
        base.Fire();

        StartCoroutine(Shots());
    }

    IEnumerator Shots()
    {
        for (int i = 0; i < shotNum; i++)
        {
            for (int j = 0; j < shotPoss.Length; j++)
            {
                Instantiate(breadPrefab, shotPoss[j].position, Quaternion.identity).TryGetComponent<Bread>(out Bread bread);

                bread.vector = transform.forward * speed;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
