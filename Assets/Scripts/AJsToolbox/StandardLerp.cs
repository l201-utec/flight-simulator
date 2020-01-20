using UnityEngine;
using System.Collections;

public class StandardLerp 
{
	

	public static IEnumerator EaseTranslate(GameObject objectToLerp, Transform destination, float time)
	{
		float elapsedTime = 0;

		while (elapsedTime < time * 3)
		{
			objectToLerp.transform.position = Vector3.Lerp (
														objectToLerp.transform.position, 
														destination.position, 
														elapsedTime/(100*time)
													);
			elapsedTime += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
		objectToLerp.transform.position = destination.position;
	}


}
