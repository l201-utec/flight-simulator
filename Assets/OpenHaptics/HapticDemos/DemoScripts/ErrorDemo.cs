using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorDemo : MonoBehaviour {

	public Text textbox = null;


	private	HapticPlugin Haptic = null;


	// Use this for initialization
	void Start () {

		Haptic = gameObject.GetComponent(typeof(HapticPlugin)) as HapticPlugin;
	}
	
	// Update is called once per frame
	void Update () {

		string content = "";

		// Retrieve a list of errors. Each item is a string containing a single error.
		string[] list = Haptic.retrieveErrorList();  

		// Dump them into a text widget with linebreaks inbetween
		for (int ii = 0; ii < list.Length; ii++)
			content += list [ii] + "\n";
		textbox.text = content;
	}
}
