using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyFollow : MonoBehaviour
{
    public GameObject airplane;
    Vector3 newPosition;

    // Update is called once per frame
    void Update()
    {
        newPosition = airplane.transform.position;
        newPosition[1] = 240;
        transform.position = newPosition;
    }
}
