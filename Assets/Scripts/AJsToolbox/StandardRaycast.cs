using UnityEngine;
using System.Collections;


public static class StandardRaycast
{

	/// <summary>
	/// Fire a raycast from an origin in any direction and length.
	/// Return "" if the raycast hit nothing, or if it hit a collider then return the collider's tag name
	/// </summary>
	public static string GetTagFromRaycast(Vector3 origin, Vector3 direction, float length)
	{
		string retVal = "";
		RaycastHit hitInfo;
		if (Physics.Raycast (origin, direction, out hitInfo, length)) {
			retVal = hitInfo.collider.tag;
		}
		return retVal;
	}


}
