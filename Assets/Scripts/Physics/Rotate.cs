using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public class Quat
    {
        // Represents w + xi + yj + zk
        public float w, x, y, z;
        public Quat(float w, float x, float y, float z)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Norm()
        {
            return Mathf.Sqrt(w * w + x * x + y * y + z * z);
        }

        public Quat Normalize()
        {
            float m = Norm();
            return new Quat(w / m, x / m, y / m, z / m);
        }

        // Returns a*b
        public static Quat Multiply(Quat a, Quat b)
        {
            float w = a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z;
            float x = a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y;
            float y = a.w * b.y + a.y * b.w - a.x * b.z + a.z * b.x;
            float z = a.w * b.z + a.z * b.w + a.x * b.y - a.y * b.x;
            return new Quat(w, x, y, z).Normalize();
        }

        public Quaternion ToUnityQuaternion()
        {
            return new Quaternion(w, x, y, z);
        }
    }

    public GameObject airplane;
    public Quat quat = new Quat(0, 0, 0, -1);
    public float speed = 5f;

    public CollisionDetect collision;
    public bool destroyed;

    public void Start()
    {
        collision = airplane.GetComponent<CollisionDetect>();
    }

    void FixedUpdate()
    {
        destroyed = collision.isDestroyed;

        if (!destroyed)
        {
            float inputX = -Input.GetAxis("Vertical");
            float inputY = Input.GetAxis("Yawn");
            float inputZ = Input.GetAxis("Horizontal");

            Quat qx = new Quat(Mathf.Cos(speed * inputX / 2), 0, 0, Mathf.Sin(speed * inputX / 2));
            Quat qy = new Quat(Mathf.Cos(speed * inputY / 2), 0, Mathf.Sin(speed * inputY / 2), 0);
            Quat qz = new Quat(Mathf.Cos(speed * inputZ / 2), Mathf.Sin(speed * inputZ / 2), 0, 0);

            quat = Quat.Multiply(qx, quat);
            quat = Quat.Multiply(qy, quat);
            quat = Quat.Multiply(qz, quat);

            airplane.transform.rotation = quat.ToUnityQuaternion();
        }
        else if (destroyed)
        {
            ResetQuat();
        }
        
    }

    public void ResetQuat()
    {
        quat = new Quat(0, 0, 0, -1);
    }

}
