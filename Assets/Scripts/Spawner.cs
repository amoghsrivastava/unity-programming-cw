using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{

    public Enemy enemy;
    [Range(0, 25)]
    public int numberOfEnemies = 6;
    private float range;


    // Start is called before the first frame update
    void Start()
    {
        range = Random.Range(0.0f, 140.0f);
        for (int index = 0; index < numberOfEnemies; index++)
        {
            Enemy spawned = Instantiate(enemy, RandomNavmeshLocation(range), Quaternion.identity);
            spawned.tag = "Enemy";
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);

        finalPosition = hit.position;
        return finalPosition;
    }
}
