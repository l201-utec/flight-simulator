using UnityEngine;
using System.Collections;

public class OrbitalVelocity : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 orbitalDirection = Quaternion.Euler (0, 90, 0) * transform.GetComponent<Gravitate> ().mainGravityDirection;
	
		transform.GetComponent<Rigidbody> ().AddForce (orbitalDirection * speed);

	}
}
