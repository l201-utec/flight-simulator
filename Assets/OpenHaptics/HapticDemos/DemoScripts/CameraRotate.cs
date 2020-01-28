using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// This is a simple utility to rotate the camera (Or anything else, really) if the haptic
// device is taken to either extreme of its X range.


public class CameraRotate : MonoBehaviour {

	public GameObject[] hapticDevices = {null, null};
	public GameObject turnTable = null;
	public float threshold = 0.1f;
	public float speed = 100f;
	public bool disableSpinner = false;

	public GameObject leftIcon = null;
	public GameObject rightIcon = null;

	public Color standardColor = Color.grey;
	public Color usedColor = Color.red;

	public GameObject cam = null;


	void Start () 
	{
		if (turnTable == null)
		{
			Debug.LogError("The 'TurnTable' object must point to the parent of the Camera and the Haptic.");
			return;
		}

	}

	// Update is called once per frame
	void Update () 
	{
		if (turnTable == null)	return;
			

		// Keypress for confused users
		if (Input.GetKeyDown("c"))
		{
			disableSpinner = !disableSpinner;
		}

		// Reset color of icons
		if (leftIcon != null)
			leftIcon.GetComponent<Image>().color = standardColor;		

		if (rightIcon != null)
			rightIcon.GetComponent<Image>().color = standardColor;		
		


		bool isDisabled = disableSpinner;

		// For each haptic device.
		for (int ii = 0; ii < hapticDevices.Length; ii++)
		{
			GameObject hapticDevice = hapticDevices [ii];
			if (hapticDevice == null)
				continue;
			if (hapticDevice.GetComponent<HapticPlugin>() == null)
			{
				Assert.IsNotNull(hapticDevice.GetComponent<HapticPlugin>());
				continue;
			}

			// Don't rotate if we're picking something up.  
			// (This seems like it might be useful, but in practice, it's just confusing.)
			HapticGrabber grabber = hapticDevice.GetComponent<HapticPlugin>().hapticManipulator.GetComponent<HapticGrabber>();
			if (grabber != null && grabber.isGrabbing())
			{
				isDisabled = true;
			}


			// Determine the X value, relative to the screen...
			float x = 0.0f;
			x = cam.GetComponent<Camera>().WorldToScreenPoint(hapticDevice.GetComponent<HapticPlugin>().stylusPositionWorld).x;
			x -= (Screen.width / 2.0f);

			// Determine the threshhold, relative to the screen.
			float T = (Screen.width / 2.0f) - Screen.width * threshold;


			// If the X value is greater than the threshold, rotate the "turnTable" object.
			if (Mathf.Abs(x) > T && !isDisabled)
			{
				float mag = Mathf.Min(Mathf.Abs(x), 10* Screen.width * threshold);
				float delta = (mag - T) * (Mathf.Abs(x) / x);
				delta /= Screen.width;

				turnTable.transform.Rotate(0, -speed * delta * Time.deltaTime, 0);

				// Light up the icon
				if (leftIcon != null && x < 0)
					leftIcon.GetComponent<Image>().color = usedColor;		

				if (rightIcon != null && x > 0)
					rightIcon.GetComponent<Image>().color = usedColor;		
			}
		}


		// If rotation is disabled, for any reason, hide the icons.
		if (leftIcon != null)
			leftIcon.SetActive(!isDisabled);

		if (rightIcon != null)
			rightIcon.SetActive(!isDisabled);

	}
}
