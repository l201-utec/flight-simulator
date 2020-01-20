using UnityEngine;
using System.Collections;

public class TurnColliderOn : MonoBehaviour {

	public float afterSeconds;

	// Use this for initialization
	void Start () {
		Invoke ("Trigger", afterSeconds);
	}

	void Trigger()
	{
		transform.GetComponent<BoxCollider> ().enabled = true;
	}

}
