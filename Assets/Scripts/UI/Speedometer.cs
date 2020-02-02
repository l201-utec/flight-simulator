using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{

    public float maxAngle;
    public float minAngle;

    public GameObject plane;
    public float speed;

    void Update()
    {
        speed = plane.GetComponent<Rigidbody>().velocity.magnitude;
        transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(minAngle, maxAngle, speed), Vector3.forward);
    }
}