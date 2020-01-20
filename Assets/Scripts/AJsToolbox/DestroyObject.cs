using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour {

	public GameObject objectToDestroy;
	public float destructionDelay;


	// Use this for initialization
	void Start () {
		Invoke ("DestroyAfterDely", destructionDelay);
	}

	void SelfDestruct()
	{
		Destroy (gameObject);
	}

}
