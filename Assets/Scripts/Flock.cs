using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockManager myManager;
    public float speed;
    bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed); 
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Bounds b = new Bounds(myManager.transform.position, myManager.roamLimits * 1);

        // If the NPC position is not in the bound
        if (!b.Contains(transform.position) || 
            Physics.Raycast(transform.position, this.transform.forward * 50, out hit)
            )
        {
            
            turning = true;
            //Debug.DrawRay(this.transform.position, this.transform.forward * 50, Color.red);
        }
        else
            turning = false;

        if (turning)
        {
            Vector3 direction = myManager.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 10)
                speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
            if (Random.Range(0, 100) < 20)
                ApplyRules();
        }

        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    // Apply flocking rules
    void ApplyRules()
    {
        GameObject[] gos;
        gos = myManager.allNPC;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoidance = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= myManager.neighbourDistance)
                {
                    vCentre += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vAvoidance = vAvoidance + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;

                }
            }
        }

        if (groupSize > 0)
        {
            vCentre = vCentre / groupSize + (myManager.goalPos - this.transform.position) ;
            speed = gSpeed / groupSize;

            Vector3 direction = (vCentre + vAvoidance) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction),
                    myManager.rotationSpeed * Time.deltaTime);

        }

    }
}
