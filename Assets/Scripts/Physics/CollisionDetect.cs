using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour {

    public GameObject explosion;

    public GameObject cube;

    public Material paint;
    public Material burnt;

    public bool isDestroyed = false;

    Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Terrain Chunk")
        {
            
            gameObject.GetComponent<Force>().enabled = false;
            //gameObject.GetComponent<Rotate>().enabled = false;

            gameObject.GetComponent<AudioSource>().enabled = false;

            if (!isDestroyed)
            {
                Instantiate(explosion, gameObject.transform);

                cube.GetComponent<MeshRenderer>().material = burnt;
            }

            //transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            //transform.GetChild(1).gameObject.SetActive(false);
            //transform.GetChild(1).GetChild(0).gameObject.SetActive(false);

            rigid.useGravity = true;

            isDestroyed = true;

        }
    }

    public void Restarting()
    {
        isDestroyed = false;

        cube.GetComponent<MeshRenderer>().material = paint;

        transform.position = new Vector3(0, 100, 0);
        transform.rotation = Quaternion.identity;

        gameObject.GetComponent<Force>().enabled = true;
        gameObject.GetComponent<AudioSource>().enabled = true;

        //gameObject.GetComponent<Rotate>().enabled = true;

        //transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        //transform.GetChild(1).gameObject.SetActive(true);
        //transform.GetChild(1).GetChild(0).gameObject.SetActive(true);

        rigid.useGravity = false;

        gameObject.GetComponent<Force>().thrust = 50f;
    }
}
