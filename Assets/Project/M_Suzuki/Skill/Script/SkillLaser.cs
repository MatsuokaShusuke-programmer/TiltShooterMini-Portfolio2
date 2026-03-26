using System.Collections;
using UnityEngine;

public class SkillLaser : Skill
{

    [SerializeField] float speed;
    [SerializeField] int shotNum;
    [SerializeField] float interval;
    [SerializeField] GameObject breadPrefab;

    [SerializeField] Transform[] shotPoss;

    private void Start()
    {
        DeadCount(Info.duration);

        StartCoroutine(Shots());
    }

    IEnumerator Shots()
    {
        while (true)
        {

            for (int i = 0; i < shotNum; i++)
            {
                for (int j = 0; j < shotPoss.Length; j++)
                {
                    Instantiate(breadPrefab, shotPoss[j].position, Quaternion.identity).TryGetComponent<Bread>(out Bread bread);

                    bread.vector = transform.up * speed;
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(interval);


        }
    }
}
