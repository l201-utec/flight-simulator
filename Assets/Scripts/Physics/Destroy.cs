using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

    public float waitTime = 3f;
	void Start () {
        Destroy(gameObject, waitTime);
	}
	
}
