using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altimeter : MonoBehaviour {

    public float setAngle;

    public GameObject plane;
    
    public float angle;

    double feet;

    void Update()
    {
        feet = plane.transform.position.y * 32.808;

        angle = 360 * (float)feet / 1000;

        transform.rotation = Quaternion.AngleAxis(-(angle-setAngle), Vector3.forward);
    }
}
