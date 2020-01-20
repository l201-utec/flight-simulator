using UnityEngine;
using System.Collections;

public class SlerpToTarget : MonoBehaviour {

	public GameObject target;
	public float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		Quaternion componentRotation = target.transform.rotation;
		componentRotation.eulerAngles = new Vector3 (transform.rotation.x, componentRotation.eulerAngles.y, transform.rotation.z);

		transform.rotation = Quaternion.Slerp (
			transform.rotation,
			componentRotation,
			Time.fixedDeltaTime * speed
		);
	}
}
