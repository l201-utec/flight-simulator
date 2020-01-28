using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HapticMouse : MonoBehaviour {


	/*
	 *  This is a very simple example of how to use the haptic device as a cursor.
	 *  I simply casts a ray to find all UI elements overlapping the stylus tip, and sends mouse-down events. Very basic.
	 */
	 
	 /* If you need a UI that fits more smoothly with the scene, you may get more elegant results if you put the UI canvas in 
	 *  the scene, attached to a 'touchable' plane, and then trigger the UI events when the stylus 'touches' it.  That will involve
	 *  a little math, but it shouldn't be too bad. And it may be worth it to give the illusion of interacting with an imaginary
	 *  touch screen.
	 */


	private	HapticPlugin Haptic = null;
	public Camera camera = null;
	public Image cursor = null;

	// Use this for initialization
	void Start () 
	{
		// find the Haptic Device
		Haptic = gameObject.GetComponent(typeof(HapticPlugin)) as HapticPlugin;
		if (Haptic == null)
			Debug.LogError("HapticMouse script must be attached to the same object as the HapticPlugin script.");

		// Find the camera
		if (camera == null)
			camera = FindObjectOfType<Camera>();
	}

	


	private bool buttonHoldDown = false; //state, so we can determine between a click and a button that's held down since last frame

	void Update () 
	{
		//This is a "click" if we're pressed now, but weren't last frame.
		bool click = false;
		if (buttonHoldDown == false && Haptic.Buttons [0] != 0)
			click = true;
		buttonHoldDown = (Haptic.Buttons [0] != 0);
			

		//Determine the screen position using the stylus position and the camera matrix transforms.
		Vector3 screenPos = camera.WorldToScreenPoint(Haptic.stylusPositionWorld);

		// In this example, the "cursor" is just a UI element.
		// Moving the system mouse cursor is more dificult, and not really recomended.
		if (cursor != null)
			cursor.rectTransform.position = screenPos;


	
		PointerEventData pointerData = new PointerEventData (EventSystem.current);
		List<RaycastResult> results = new List<RaycastResult> ();

		// Perform a raycast to get a list of all elements under the cursor.
		pointerData.position = screenPos;
		EventSystem.current.RaycastAll(pointerData, results);

		// Now that we've found the things. Let's select them ...
		bool selectedAtLeastOneThing = false;
		foreach(RaycastResult R in results)
		{
			GameObject go = R.gameObject;
			Selectable S = go.GetComponent <Selectable>();
			if (S != null)
			{
				S.Select();
				selectedAtLeastOneThing = true;
			}

			// If we've found a button, we can click on it.
			Button B = go.GetComponent<Button>();
			if (B != null)
			{
				if (click)
				{
					B.onClick.Invoke();
					B.Invoke("OnPointerDown",0.0f);
				} 
				if (buttonHoldDown == false)
				{
					B.Invoke("OnPointerUp",0.0f);
				}
			}

		}

		//If we're not hovering over anything, deselect everything.
		// NOTE : This is pretty crude, and will interfere with anyone trying to operate the UI via keyboard.
		if (selectedAtLeastOneThing == false)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

}
