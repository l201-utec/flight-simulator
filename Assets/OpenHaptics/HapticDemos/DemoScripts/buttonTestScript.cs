using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonTestScript : MonoBehaviour {

	public GameObject panel = null;

	public void testButton() {
		Debug.Log("BUTTON PRESSED!");
	}
		
	public void testButtonRed()
	{
		if (panel != null)
			panel.GetComponent<Image>().color = Color.red;
	}
	public void testButtonGreen()
	{
		if (panel != null)
			panel.GetComponent<Image>().color = Color.green;
	}
	public void testButtonBlue()
	{
		if (panel != null)
			panel.GetComponent<Image>().color = Color.blue;
	}
}
