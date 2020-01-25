using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour {

    public float maxAngle;
    public float minAngle;

    public GameObject plane;
    private Force speed;

    public float ratio;

	void Start () {
        speed = plane.GetComponent<Force>();

	}
	
	// Update is called once per frame
	void Update () {
        ratio = speed.thrust / speed.maxSpeed;

        transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(minAngle, maxAngle, ratio), Vector3.forward);
	}
}
