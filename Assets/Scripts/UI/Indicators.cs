using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Indicators : MonoBehaviour {

    public GameObject plane;

    public Text speed;
    public Text altitude;
    public Text orientation;
    public Text compass;

    public Text anglex;
    public Text angley;
    public Text anglez;

    public Text vecx;
    public Text vecy;
    public Text vecz;


    private Force force;
    private CollisionDetect collision;

    public bool destroy;

    //private Dictionary<float, string> dic = new Dictionary<float, string>() { { 0, "N" }, { 90, "E" }, { 180, "S" }, { 270, "O" } };

    private void Start()
    {
        force = plane.GetComponent <Force> ();
        collision = plane.GetComponent<CollisionDetect>();
                
    }

    void Update () {

        destroy = collision.isDestroyed;

        float or = plane.transform.eulerAngles.y;

        speed.text = (force.thrust * 1.9438).ToString("0");
        altitude.text = (plane.transform.position.y * 32.808 <= 9999 ? plane.transform.position.y * 3.2808 * 10 : 9999).ToString("0");
        orientation.text = or.ToString("0");

        if (or >= 1 && or < 90)
        {
            compass.text = "N";
        }

        else if (or >= 90 && or < 180)
        {
            compass.text = "E";
        }
        
        else if (or >= 180 && or < 270)
        {
            compass.text = "S";
        }

        else if (or >= 270 && or <= 360)
        {
            compass.text = "W";
        }

        if (destroy)
        {
            anglex.text = 0.ToString("0.00");
            angley.text = 0.ToString("0.00");
            anglez.text = 0.ToString("0.00");

            gameObject.SendMessage("Zeros");

        }
        else if (!destroy)
        {
            anglex.text = plane.transform.eulerAngles.x.ToString("0.00");
            angley.text = plane.transform.eulerAngles.y.ToString("0.00");
            anglez.text = plane.transform.eulerAngles.z.ToString("0.00");

            gameObject.SendMessage("Angles");
        }
        
    }

}
