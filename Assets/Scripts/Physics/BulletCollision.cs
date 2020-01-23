using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        //if (collision.gameObject.name == "MyGameObjectName")
        //{
        //    //If the GameObject's name matches the one you suggest, output this message in the console
        //    Debug.Log("Do something here");
        //}

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "LandTile")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("Do something else here");
            Destroy(gameObject);
        }
    }
}