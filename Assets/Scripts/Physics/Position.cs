using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class Lift : MonoBehaviour {

    Vector2 mouselook;
    Vector2 smooth;

    Vector3 x_axis;
    Vector3 y_axis;
    Vector3 z_axis;

    public Transform player;

    public float valueinx;
    public float valueinz;

    public float sensitivity = 5.0f;
    public float smoothing = 2.0f;

    public float[,,] main_matrix;

    public float[,,] rot_matrix_x;
    public float[,,] rot_matrix_y;
    public float[,,] rot_matrix_z;


    public float[,,] new_m;

    private void Start()
    {
        main_matrix = new float[,,] { 
            {
                { 1, 0, 0 }, 
                { 0, 1, 0 }, 
                { 0, 0, 1 }
            }
        };

        rot_matrix_x = new float[,,] {
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            }
        };
        rot_matrix_y = new float[,,] {
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            }
        };

        rot_matrix_z = new float[,,] {
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            }
        };

        Set_Vector();

    }

    void Update()
    {
        
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));


        Debug.Log("Input Raw: " + input.x + " " + input.y);

        input = Vector2.Scale(input, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smooth.x = Mathf.Lerp(smooth.x, input.x, 1f / smoothing);
        smooth.y = Mathf.Lerp(smooth.y, input.y, 1f / smoothing);
        mouselook += smooth;

        Debug.Log("Procs. Input: " + mouselook.x + " " + mouselook.y);

        valueinx = mouselook.x;
        valueinz = mouselook.y;
        
        if (input.x != 0)
        {

            //temp = x_axis;

            //Quaternion.AngleAxis(mouselook.x, Vector3.right);

            rot_matrix_x = new float[,,] {
            {
                { 1, 0, 0 },
                { 0, Mathf.Cos(mouselook.x * Mathf.PI/180),-Mathf.Sin(mouselook.x * Mathf.PI/180)},
                { 0, Mathf.Sin(mouselook.x* Mathf.PI/180), Mathf.Cos(mouselook.x * Mathf.PI/180)},
            }
        };

            Multiply(main_matrix, rot_matrix_x);
            Multiply(main_matrix, rot_matrix_y);
            Multiply(main_matrix, rot_matrix_z);

            Set_Vector();
            
        }
        
        if (input.y != 0)
        {

            //temp = x_axis;

            Quaternion.AngleAxis(mouselook.y, Vector3.forward);

            rot_matrix_z = new float[,,] {
            {
                {Mathf.Cos(mouselook.y * Mathf.PI/180), -Mathf.Sin(mouselook.y * Mathf.PI/180) , 0 },
                {Mathf.Sin(mouselook.y * Mathf.PI/180), Mathf.Cos(mouselook.y * Mathf.PI/180) , 0},
                {0, 0, 1},
            }
        };

            Multiply(main_matrix, rot_matrix_x);
            Multiply(main_matrix, rot_matrix_y);
            Multiply(main_matrix, rot_matrix_z);

            Set_Vector();
        }

        else
        {
            Debug.Log("Matrix in X: " + main_matrix[0, 0, 0] + " " + main_matrix[0, 0, 1] + " " + main_matrix[0, 0, 2]);
            Debug.Log("Matrix in Y: " + main_matrix[0, 1, 0] + " " + main_matrix[0, 1, 1] + " " + main_matrix[0, 1, 2]);
            Debug.Log("Matrix in Z: " + main_matrix[0, 2, 0] + " " + main_matrix[0, 2, 1] + " " + main_matrix[0, 2, 2]);
        }
    }

    void Set_Vector()
    {
        x_axis.x = main_matrix[0, 0, 0];
        x_axis.y = main_matrix[0, 0, 1];
        x_axis.z = main_matrix[0, 0, 2];

        y_axis.x = main_matrix[0, 1, 0];
        y_axis.y = main_matrix[0, 1, 1];
        y_axis.z = main_matrix[0, 1, 2];

        z_axis.x = main_matrix[0, 2, 0];
        z_axis.y = main_matrix[0, 2, 1];
        z_axis.z = main_matrix[0, 2, 2];
    }

    void Multiply(float [,,] matrix, float [,,] matrix2)
    {

        int i, j, k, n,p;

        new_m = new float[,,]
        {
            {
                {0, 0, 0},
                {0, 0, 0},
                {0, 0, 0}
            }
        };



        for (i = 0; i < 3; ++i)
        {
            for (j = 0; j < 3; ++j)
            {
                for (k = 0; k < 3; ++k)
                {
                    new_m[0,i,j] += matrix[0,i,k] * matrix2[0,k,j];
                }

            }
        }

        for (n = 0; n < 3; ++n)
        {
            for (p = 0; p < 3; ++p)
            {
                matrix[0, n,p] = new_m[0, n, p];
            }
        }

    }


}
