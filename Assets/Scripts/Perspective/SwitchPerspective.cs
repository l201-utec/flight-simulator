using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPerspective : MonoBehaviour
{
    public Camera cam1;
    public Camera cam2;
    public GameObject planeInterface1;
    public GameObject planeInterface2;
    
    // Start is called before the first frame update
    void Start()
    {
        cam1.enabled = true;
        planeInterface1.SetActive(true);
        cam2.enabled = false;
        planeInterface2.SetActive(false);
        cam2.GetComponent<AudioListener>().enabled = !cam2.GetComponent<AudioListener>().enabled;
        (cam2.GetComponent("SmoothMouseLook") as MonoBehaviour).enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            cam1.enabled = !cam1.enabled;
            cam1.GetComponent<AudioListener>().enabled = !cam1.GetComponent<AudioListener>().enabled;
            planeInterface1.SetActive(!planeInterface1.activeSelf);
            cam2.enabled = !cam2.enabled;
            cam2.GetComponent<AudioListener>().enabled = !cam2.GetComponent<AudioListener>().enabled;
            (cam2.GetComponent("SmoothMouseLook") as MonoBehaviour).enabled = !((cam2.GetComponent("SmoothMouseLook") as MonoBehaviour).enabled);
            planeInterface2.SetActive(!planeInterface2.activeSelf);
        }
    }
}