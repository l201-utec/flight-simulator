using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection : MonoBehaviour
{
    public GameObject smoke;
    public GameObject explosion;
    public int collisionCount = 0;

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            var colObj = contact.thisCollider;
            GameObject planeSmoke = Instantiate(smoke, colObj.transform.position, colObj.transform.rotation);
            planeSmoke.transform.parent = transform;
            collisionCount++;

            // Debug.LogWarning($"Counts: {collisionCount}");

            if (collisionCount >= 3 || collision.gameObject.CompareTag("LandTile"))
            {
                Instantiate(explosion, colObj.transform.position, colObj.transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}
