using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
        
    public GameObject Plane;
    public GameObject Controller;

    private Menu menu;

    public float thrust = 50f;
    public float modifier = 2f;
    public float brakeModifier = 2f;
    public float brakesSen = 0.3f;

    public float maxSpeed = 150f;
    public float minSpeed = 15f;
    public float terminalSpeed = 115f;

    private void Start()
    {
        menu = Controller.GetComponent<Menu>();

    }

    void Update()
    {
        if (!menu.gamePaused)
        {
            thrust -= transform.forward.y * Time.deltaTime * thrust;

            if (thrust < minSpeed)
            {
                thrust = minSpeed;
            }

            else if (thrust > maxSpeed)
            {
                thrust = maxSpeed;
            }

            if (Input.GetKey("joystick button 1") || Input.GetKey("space"))
            {
                thrust -= brakeModifier;
                transform.Translate(Vector3.forward * thrust * Time.deltaTime);

            }

            else
            {
                if (thrust >= terminalSpeed)
                {
                    transform.Translate(Vector3.forward * thrust * Time.deltaTime);
                }

                else
                {
                    thrust += modifier;
                    transform.Translate(Vector3.forward * thrust * Time.deltaTime);
                }

            }

        }
    }
        
        
       
}
    
