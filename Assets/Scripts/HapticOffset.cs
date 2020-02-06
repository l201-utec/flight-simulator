using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticOffset : MonoBehaviour
{
    public Vector3 offset;
    Vector3 realPosition;
    Quaternion realRotation;

    void Update()
    {
        realPosition = GameObject.Find("HapticDevice").GetComponent<HapticPlugin>().stylusPositionWorld;
        realRotation = GameObject.Find("HapticDevice").GetComponent<HapticPlugin>().stylusRotationWorld;
        transform.position = realPosition + offset;
        transform.rotation = realRotation;
    }
}
