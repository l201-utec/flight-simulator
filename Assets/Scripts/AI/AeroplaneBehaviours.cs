using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroplaneBehaviours : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject targetObj;
    public GameObject targetTracker;
    public GameObject targetTracker2;
    public float distanceBetween;
    float predictionTime = 2f;

    void Start()
    {
        if (targetObj == null)
        {
            targetObj = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Pursue(targetObj.transform.position);
    }

    void Seek(Vector3 target)
    {

        // Modify target so as not to collide
        bool aboutToCollide = false;

        if (aboutToCollide)
        {
            
        }

        // Set waypoint to target
        targetTracker.transform.position = target;
        targetTracker2.transform.position = target;
    }

    void Pursue(Vector3 target)
    {
        // get better prediction
        Vector3 futurePosition = targetObj.transform.position + 
                (targetObj.transform.forward * (Time.deltaTime * predictionTime));

        Seek(futurePosition);
    }
}
