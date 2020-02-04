using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticOffset : MonoBehaviour
{
    public Vector3 offset;
    Vector3 rawPosition;

    void Update()
    {
        rawPosition = GameObject.Find("HapticDevice").GetComponent<HapticPlugin>().stylusPositionWorld;
        rawPosition += offset;
    }
}
