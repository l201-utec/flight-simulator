using UnityEngine;
using System.Collections;

public class LerpToPosition : MonoBehaviour {

	public GameObject target;
	public float speed, height;



	void Start()
	{
		StartCoroutine(StandardLerp.EaseTranslate (gameObject, target.transform, speed));
		//Debug.Log ("test");
	}

	/*
	void FixedUpdate() {
		transform.position = Vector3.Lerp (
			transform.position,
			target.transform.position + new Vector3(0,height,0),
			Time.fixedDeltaTime * speed
		);
	}
	*/


	void Update()
	{

		
	}

}
