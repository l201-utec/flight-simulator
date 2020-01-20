using UnityEngine;
using System.Collections;

public class Gravitate : MonoBehaviour {


	public Vector3 mainGravityDirection;
	public GameObject objectToGravitateToward;
	public float massMultiplier;

	void Update () {

		mainGravityDirection = (objectToGravitateToward.transform.position - transform.position).normalized;
		transform.GetComponent<Rigidbody> ().AddForce (mainGravityDirection * objectToGravitateToward.transform.GetComponent<Rigidbody>().mass * massMultiplier);
	}
}
