using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.IO.Ports;

public class Commun : MonoBehaviour
{
    public string message_recv;
    public string message_send;
    
    public Text angleX;
    public Text angleY;
    public Text angleZ;

    private int byte_recv;
    private bool awaitingResponse = false;
    SerialPort sp = new SerialPort("COM6", 115200);

    public void Start()
    {
        //sp.Close();
        sp.Open();

        if (sp.IsOpen)
        {
            Debug.Log("SERIAL IS OPEN");
        }
        
        sp.ReadTimeout = 1000;
        sp.WriteTimeout = 1000;
    }

    public void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                //sp.ReadByte();
                if(!awaitingResponse)
                {
                    Debug.Log("SER: " + message_send);
                    sp.Write(message_send);
                    sp.BaseStream.Flush();
                    awaitingResponse = true;

                }
                if (sp.BytesToRead > 0)
                {
                    byte_recv = sp.ReadByte();
                    awaitingResponse = false;
                }


                //text.text = message_recv;
            }
            catch (System.Exception)
            {
                sp.Close();
                throw;
            }
        }
    }

    public void Zeros()
    {
        message_send = "0$0$0\n";
        //Debug.Log("RECIEVED ZEROS");
    }

    public void Angles()
    {
        message_send = angleZ.text  + "$" + angleY.text   + "$" + angleX.text  + "\n";
        //Debug.Log("RECIEVED ANGLES");
    }

    public void TransmitAngles(string x, string y, string z)
    {
        message_send = x + "$" + y + "$" + z + "\n";
    }   
}

/*
using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Linq;

public class Interface : MonoBehaviour
{
    SerialPort sp;

    void Start()
    {
        sp = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One); //Replace "COM4" with whatever port your Arduino is on.
        sp.DtrEnable = false; //Prevent the Arduino from rebooting once we connect to it. 
                              //A 10 uF cap across RST and GND will prevent this. Remove cap when programming.
        sp.ReadTimeout = 1; //Shortest possible read time out.
        sp.WriteTimeout = 1; //Shortest possible write time out.
        sp.Open();
        if (sp.IsOpen)
            sp.Write("Hello World");
        else
            Debug.LogError("Serial port: " + sp.PortName + " is unavailable");
        sp.Close(); //You can't program the Arduino while the serial port is open, so let's close it.
    }
}*/