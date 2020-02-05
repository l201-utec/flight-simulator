using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0;i<20; i++)
        {
            if (Input.GetKeyDown("joystick button " + i))
             {
            Debug.Log("Button " + i + " was pressed!");
             }
        }
        
    }
}
