using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    public GameObject AircraftJet;
    public float deltaAngle = .5f;
    private  float steeringWheelAngle = 0f;

    private float axisValue;
    void Update()
    {
        
        axisValue = Input.GetAxis("Horizontal");

        //Debug.LogWarning(axisValue);


        if (axisValue <= .05f && axisValue >= -.05f)
        {

            if (steeringWheelAngle < 180f && steeringWheelAngle > 0f)
                steeringWheelAngle -= deltaAngle;
            else if (steeringWheelAngle >= 180 && steeringWheelAngle <= 360)
                steeringWheelAngle += deltaAngle;

        }
        else
        {
            steeringWheelAngle = AircraftJet.transform.eulerAngles.z;
            
        }
        
        transform.localRotation = Quaternion.AngleAxis(steeringWheelAngle, Vector3.forward);
    

	}
}
