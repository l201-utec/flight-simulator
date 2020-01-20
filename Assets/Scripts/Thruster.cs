using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour {

	public bool on;
	public float power;


	void Update () {

		if (Input.GetButton("Jump")) {

			Debug.Log ("what the fuck?");
			on = true;
		
		} else  {
			on = false;
		}

		if (Input.GetAxis ("Horizontal") != null) {
			float direction = Input.GetAxis ("Horizontal");
			transform.Rotate (direction,0,0);
		}

		if (on) {
			transform.GetComponent<Rigidbody> ().AddForce (transform.up * power);

		}
	}
}
