using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;

public class Car : MonoBehaviour
{
    public WheelCollider backLeft, backRight, frontLeft, frontRight;
    public Transform backLeftTrans, backRightTrans, frontLeftTrans, frontRightTrans;

    float _angle = 25f;
    float _motorSpeed = 100;

    float h, v;

    private TcpClient tcpClient;
    private StreamWriter writer;
    private StreamReader reader;

    bool idDriving = true;

    void Start()
    {
        ConnectToESP32();
        Task.Run(() => CheckPIRStatusPeriodically());
    }

    async void CheckPIRStatusPeriodically()
    {
        while (true)
        {
            //await Task.Delay(500);
            RequestPIRStatus();
        }
    }

    void ConnectToESP32()
    {
        tcpClient = new TcpClient("192.168.4.1", 80);

        if (tcpClient.Connected)
        {
            Debug.Log("Connected to ESP32");
            writer = new StreamWriter(tcpClient.GetStream());
            reader = new StreamReader(tcpClient.GetStream());
        }
    }

    void RequestPIRStatus()
    {
        if (tcpClient != null && tcpClient.Connected)
        {
            string response = reader.ReadLine();

            if (response != null)
            {
                if (response.Contains("Respond:1"))
                {
                    Debug.Log("Motion detected in Unity!");
                    idDriving = false;
                }
                else if (response.Contains("Respond:0"))
                {
                    Debug.Log("No motion detected in Unity.");
                    idDriving = true;
                }
            }
        }
    }

    void OnDestroy()
    {
        if (tcpClient != null)
            tcpClient.Close();
    }

    void Update()
    {
        h = 0;
        v = 1;
        Debug.Log(idDriving);
        frontLeft.steerAngle = _angle * h;
        frontRight.steerAngle = _angle * h;

        if (idDriving)
        {
            Drive();
        }
        else
        {
            Brake();

        }
        UpdateWheel(backLeft, backLeftTrans);
        UpdateWheel(backRight, backRightTrans);
        UpdateWheel(frontLeft, frontLeftTrans);
        UpdateWheel(frontRight, frontRightTrans);
    }

    void Drive()
    {
        backLeft.brakeTorque = 0;
        backRight.brakeTorque = 0;
        backLeft.motorTorque = _motorSpeed * v;
        backRight.motorTorque = _motorSpeed * v;
    }

    void Brake()
    {
        backLeft.brakeTorque = 50;
        backRight.brakeTorque = 50;
    }

    void UpdateWheel(WheelCollider col, Transform t)
    {
        Vector3 position = t.position;
        Quaternion rotation = t.rotation;

        col.GetWorldPose(out position, out rotation);

        t.position = position;
        t.rotation = rotation;
    }
}
