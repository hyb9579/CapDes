using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;

public class mymanager : MonoBehaviour
{
    private BluetoothHelper helper;

    private string deviceName;
    void Start()
    {
        deviceName = "ARcon_1";
        try
        {
            helper = BluetoothHelper.GetInstance(deviceName);
            helper.OnConnected += OnConnected;
            helper.OnConnectionFailed += OnConnFailed;

            helper.setTerminatorBasedStream("\n");
            //or
            //helper.setLengthBasedStream();

            if (helper.isDevicePaired())
            {
                helper.Connect();
            }
        }catch(BluetoothHelper.BlueToothNotEnabledException ex) { }
        catch(BluetoothHelper.BlueToothNotReadyException ex) { }
        catch(BluetoothHelper.BlueToothPermissionNotGrantedException ex) { }
    }

    void OnConnected()
    {
        helper.StartListening();

        helper.SendData("Hi arduino!");
    }

    void OnConnFailed()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (helper.Available)
        {
            string msg = helper.Read();
        }
    }
}
