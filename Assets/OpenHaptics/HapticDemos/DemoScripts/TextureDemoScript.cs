using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TextureDemoScript : MonoBehaviour 
{

	public Texture2D FrictionTexture = null;

	// Keep track of the Haptic Devices
	HapticPlugin device = null;
	float luminocity = 0.5f;

	[Header("White Surface")]
	[Range(0.0f, 1.0f)] public float hlStiffness = 0.7f;
	[Range(0.0f, 1.0f)] public float hlDamping = 0.1f;
	[Range(0.0f, 1.0f)] public float hlStaticFriction = 0.2f;
	[Range(0.0f, 1.0f)] public float hlDynamicFriction = 0.3f;
	[Range(0.0f, 1.0f)] public float hlPopThrough = 0.0f;

	[Header("Black Surface")]
	[Range(0.0f, 1.0f)] public float hlStiffness2 = 0.7f;
	[Range(0.0f, 1.0f)] public float hlDamping2 = 0.1f;
	[Range(0.0f, 1.0f)] public float hlStaticFriction2 = 0.7f;
	[Range(0.0f, 1.0f)] public float hlDynamicFriction2 = 0.9f;
	[Range(0.0f, 1.0f)] public float hlPopThrough2 = 0.0f;


	// Use this for initialization
	void Start () 
	{
		device = (HapticPlugin) Object.FindObjectOfType(typeof(HapticPlugin));
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Find the pointer to the collider that defines the "zone". 
		Collider collider = gameObject.GetComponent<Collider>();
		if (collider == null)
		{
			Debug.LogError("This Haptic Surface Demo Effect requires a collider");
			return;
		}

		if (FrictionTexture == null)
		{
			Debug.LogError("This Haptic Surface Demo Effect requires a texture assigned to it.");
			return;
		}



		Vector3 StylusPos = device.stylusPositionWorld;	//World Coordinates
		Vector3 CP = collider.ClosestPointOnBounds(StylusPos); 	//World Coordinates
		float delta = (CP - StylusPos).magnitude;

		if (delta < 1.0f)
		{
			Vector3 direction =  transform.position - CP;
			direction.Normalize();


			// Cast a ray between the stylus and the center of the collider
			RaycastHit[] hits = Physics.RaycastAll(CP, direction);

			//There may be some false positives in the list, so loop through
			//and find the hit on the current collider.
			foreach (RaycastHit H in hits)
			{
				if (H.collider == collider)
				{
					// This is the correct hit, so retrieve the UV values...
					Vector2 UV = H.textureCoord;

					// Scale the UV to the size of the texture...
					int U = (int)(UV.x * FrictionTexture.width);
					int V = (int)(UV.y * FrictionTexture.height);


					// Retrieve the color of that pixel.
					Color C = FrictionTexture.GetPixel(U, V);
					luminocity = C.grayscale;
					break;
				}
			}

			// Assign the haptic material setttings so that they transition proportionatly
			// between the two values based on the luminocity of the texel. 
			float Value = luminocity;
			float inVal = 1.0f-Value;

			HapticPlugin.shape_settings(gameObject.GetInstanceID(),
				hlStiffness * Value + hlStiffness2 * inVal,
				hlDamping * Value + hlDamping2 * inVal,
				hlStaticFriction * Value + hlStaticFriction2 * inVal,
				hlDynamicFriction * Value + hlDynamicFriction2 * inVal,
				hlPopThrough * Value + hlPopThrough2 * inVal);



		}



	}
}
