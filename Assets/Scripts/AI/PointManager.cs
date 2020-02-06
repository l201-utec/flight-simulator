using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    // Start is called before the first frame update

    public int spaceBetweenPoints = 10;
    public int lenght_x;
    public int lenght_y;
    public int lenght_z;

    public GameObject target;


    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void FixedUpdate()
    {
        Vector3 center = target.transform.position;


        
    }
}
