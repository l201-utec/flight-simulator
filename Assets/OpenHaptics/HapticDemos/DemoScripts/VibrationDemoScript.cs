using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationDemoScript : MonoBehaviour {

	public HapticPlugin HapticDevice = null;
	private bool vibrationOn;
	private int FXID = -1;

	void Start () 
	{
		vibrationOn = false;
		if (HapticDevice == null)
			HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));
		
		if( HapticDevice /* STILL */ == null )
			Debug.LogError("This script requires that Haptic Device be assigned.");
	}

	void TurnEffectOn()
	{
		if (HapticDevice == null) return; 		//If there is no device, bail out early.

		// If a haptic effect has not been assigned through Open Haptics, assign one now.
		if (FXID == -1)
		{
			FXID = HapticPlugin.effects_assignEffect(HapticDevice.configName);

			if (FXID == -1) // Still broken?
			{
				Debug.LogError("Unable to assign Haptic effect.");
				return;
			}
		}

		// Send the effect settings to OpenHaptics.
		double[] pos = {0.0, 0.0, 0.0}; // Position (not used for vibration)
		double[] dir = {0.0, 1.0, 0.0}; // Direction of vibration

		HapticPlugin.effects_settings(
			HapticDevice.configName,
			FXID,
			0.33, // Gain
			0.33, // Magnitude
			300,  // Frequency
			pos,  // Position (not used for vibration)
			dir); //Direction.
		
		HapticPlugin.effects_type( HapticDevice.configName,	FXID,4 ); // Vibration effect == 4

		HapticPlugin.effects_startEffect(HapticDevice.configName, FXID );
	}

	void TurnEffectOff()
	{
		if (HapticDevice == null) return; 		//If there is no device, bail out early.
		if (FXID == -1)	return;  				//If there is no effect, bail out early.

		HapticPlugin.effects_stopEffect(HapticDevice.configName, FXID );
	}


	void Update () 
	{
		// If there's no haptic device, bail out early.
		if (HapticDevice == null) return;


		// Toggle on the v Key
		if (Input.GetKeyDown("v"))
		{
			vibrationOn = !vibrationOn;

			//If we've just turned it ON
			if (vibrationOn)
				TurnEffectOn();
			else
				TurnEffectOff();
		}

	}

	void OnDestroy()
	{
		TurnEffectOff();
	}

	void OnApplicationQuit()
	{
		TurnEffectOff();
	}

	void OnDisable()
	{
		TurnEffectOff();
	}

	/*
	void Update()
	{
		// If there's no haptic device, bail out early.
		if (HapticDevice == null)
			return;

		bool buttonState = (HapticDevice.Buttons [0] != 0);

		//If the Button is on and the vibration isn't, or vice-versa
		if ( buttonState != vibrationOn)
		{
			vibrationOn = buttonState;

			//If we've just turned it ON
			if (vibrationOn)
				TurnEffectOn();
			else
				TurnEffectOff();
		}

	}
	*/
}

