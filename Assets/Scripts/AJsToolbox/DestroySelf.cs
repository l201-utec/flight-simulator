using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour {

	public float timeUntilDestruction;


	// Use this for initialization
	void Start () {
		Invoke ("SelfDestruct", timeUntilDestruction);
	}

	void SelfDestruct()
	{
		Destroy (gameObject);
	}

}
