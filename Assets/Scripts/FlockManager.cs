using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject npcPrefab;
    public int numNPC = 20;
    public GameObject[] allNPC;
    public Vector3 roamLimits = new Vector3(40, 0, 40);

    public Vector3 goalPos;

    [Header("NPC Speed Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
    public float neighbourDistance;             // In what range an NPC considers its neighbors
    [Range(0.0f, 5.0f)]
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        allNPC = new GameObject[numNPC];
        for (int i = 0; i < numNPC; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-roamLimits.x, roamLimits.x), 
                0, 
                Random.Range(-roamLimits.z, roamLimits.z));
            allNPC[i] = (GameObject)Instantiate(npcPrefab, pos, Quaternion.identity);
            allNPC[i].GetComponent<Flock>().myManager = this;
        }
        goalPos = this.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0, 100) < 2)
        {
            goalPos = this.transform.position + new Vector3(Random.Range(-roamLimits.x, roamLimits.x),
                0,
                Random.Range(-roamLimits.z, roamLimits.z));
        }
    }
}
