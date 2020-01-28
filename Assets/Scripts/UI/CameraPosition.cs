using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {

    public GameObject plane;
    public bool firstView;

    void Update () {

        if (firstView == true) {

            transform.rotation = plane.transform.rotation;

                }
        
        transform.LookAt(plane.transform.position);

        if(Input.GetKey("joystick button 4"))
        {
            OnClick();
        }

        if (Input.GetKey("joystick button 5"))
        {
            OnSecondClick();
        }
    }

    public void OnClick()
    {
        transform.localPosition = new Vector3 (0,0,0) ;
        firstView = true;


    }

    public void OnSecondClick()
    {
        transform.localPosition = new Vector3(0, (float) 1.9, (float) -10.3);
        firstView = false;
    }
    
}
