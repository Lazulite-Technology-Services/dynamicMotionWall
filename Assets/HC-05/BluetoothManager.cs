﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ArduinoBluetoothAPI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BluetoothManager : MonoBehaviour
{
    // Use this for initialization
    BluetoothHelper bluetoothHelper;

    // BluetoothHelper bluetoothHelper2;
    string deviceName = "HC-05";

    //public Text text;
    public TextMeshProUGUI text;

    string received_message;

    public static BluetoothManager Instance;

    private void Awake()
    {
        Instance = this;
        //PlayerPrefs.SetString("BT_Device", deviceName);
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("BT_Device"))
        {
            deviceName = PlayerPrefs.GetString("BT_Device"); //bluetooth should be turned ON;
        }

        try
        {
            deviceName = "HC-05";
            bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data

            //bluetoothHelper.setFixedLengthBasedStream(3); //receiving every 3 characters together
            bluetoothHelper.setTerminatorBasedStream("\n"); //delimits received messages based on \n char

            //if we received "Hi\nHow are you?"
            //then they are 2 messages : "Hi" and "How are you?"
            // bluetoothHelper.setLengthBasedStream();
            /*
			will received messages based on the length provided, this is useful in transfering binary data
			if we received this message (byte array) :
			{0x55, 0x55, 0, 3, 'a', 'b', 'c', 0x55, 0x55, 0, 9, 'i', ' ', 'a', 'm', ' ', 't', 'o', 'n', 'y'}
			then its parsed as 2 messages : "abc" and "i am tony"
			the first 2 bytes are the length data writted on 2 bytes
			byte[0] is the MSB
			byte[1] is the LSB

			on the unity side, you dont have to add the message length implementation.

			if you call bluetoothHelper.SendData("HELLO");
			this API will send automatically :
			 0x55 0x55    0x00 0x05   0x68 0x65 0x6C 0x6C 0x6F
			|________|   |________|  |________________________|
			 preamble      Length             Data

			
			when sending data from the arduino to the bluetooth, there's no preamble added.
			this preamble is used to that you receive valid data if you connect to your arduino and its already send data.
			so you will not receive 
			on the arduino side you can decode the message by this code snippet:
			char * data;
			char _length[2];
			int length;

			if(Serial.avalaible() >2 )
			{
				_length[0] = Serial.read();
				_length[1] = Serial.read();
				length = (_length[0] << 8) & 0xFF00 | _length[1] & 0xFF00;

				data = new char[length];
				int i=0;
				while(i<length)
				{
					if(Serial.available() == 0)
						continue;
					data[i++] = Serial.read();
				}


				...process received data...


				delete [] data; <--dont forget to clear the dynamic allocation!!!
			}
			*/
            LinkedList<BluetoothDevice> ds =
                bluetoothHelper.getPairedDevicesList();

            if (bluetoothHelper.isDevicePaired())
            {
				text.text = "Connecting to " + deviceName;
                bluetoothHelper.Connect();
            }

            CancelInvoke("CheckConnection");
            InvokeRepeating("CheckConnection",5f,10f);

            Debug.Log (ds);
            // if(bluetoothHelper.isDevicePaired())
            // 	sphere.GetComponent<Renderer>().material.color = Color.blue;
            // else
            // 	sphere.GetComponent<Renderer>().material.color = Color.grey;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            text.text = ex.Message;
            //BlueToothNotEnabledException == bluetooth Not turned ON
            //BlueToothNotSupportedException == device doesn't support bluetooth
            //BlueToothNotReadyException == the device name you chose is not paired with your android or you are not connected to the bluetooth device;
            //								bluetoothHelper.Connect () returned false;
        }
    }

    private void OnApplicationQuit()
    {
        CancelInvoke("CheckConnection");
    }

    private void OnEnable()
    {
        CancelInvoke("CheckConnection");
        InvokeRepeating("CheckConnection", 5f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        /*
		//Synchronous method to receive messages
		if(bluetoothHelper != null)
		if (bluetoothHelper.Available)
			received_message = bluetoothHelper.Read ();
		*/
    }

    //Asynchronous method to receive messages
    void OnMessageReceived()
    {
        //StartCoroutine(blinkSphere());
        received_message = bluetoothHelper.Read();
        Debug.Log (received_message);
        text.text = received_message;
        // Debug.Log(received_message);
    }

    public void SendBTMessage(string message)
    {
        if (bluetoothHelper.isConnected())
        {
            bluetoothHelper.SendData (message);
        }
    }

    public void TryReconnectBluetooth()
    {
        text.text = "Trying Reconnecting to " + deviceName;
        if (bluetoothHelper.isDevicePaired())
        {   
            bluetoothHelper.Disconnect();
            bluetoothHelper.Connect();
        }
    }

    void OnConnected()
    {
        try
        {
            bluetoothHelper.StartListening();
            text.text = "Connected Successfully " + deviceName;
            // bluetoothHelper2 = BluetoothHelper.GetNewInstance();
            // bluetoothHelper2.OnScanEnded += ScanEnded2;
            // bluetoothHelper2.ScanNearbyDevices();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // private void ScanEnded2(LinkedList<BluetoothDevice> devices){
    //     Debug.Log(devices.Count);
    // }
    void OnConnectionFailed()
    {
        Debug.Log("Connection Failed, retrying");
        text.text = "Connection Failed with " + deviceName;
        TryReconnectBluetooth();
    }

    void CheckConnection()
    {
        //Debug.Log(bluetoothHelper.isConnected());
        if (!bluetoothHelper.isConnected())
        {
            TryReconnectBluetooth();
        }
    }

    // //Call this function to emulate message receiving from bluetooth while debugging on your PC.
    // void OnGUI()
    // {
    // 	if(bluetoothHelper!=null)
    // 		bluetoothHelper.DrawGUI();
    // 	else

    // 	return;
    // 	if(!bluetoothHelper.isConnected())
    // 	if(GUI.Button(new Rect(Screen.width/2-Screen.width/10, Screen.height/10, Screen.width/5, Screen.height/10), "Connect"))
    // 	{
    // 		if(bluetoothHelper.isDevicePaired())
    // 			bluetoothHelper.Connect (); // tries to connect
    // 	}
    // 	if(bluetoothHelper.isConnected())
    // 	if(GUI.Button(new Rect(Screen.width/2-Screen.width/10, Screen.height - 2*Screen.height/10, Screen.width/5, Screen.height/10), "Disconnect"))
    // 	{
    // 		bluetoothHelper.Disconnect ();
    // 	}
    // 	if(bluetoothHelper.isConnected())
    // 	if(GUI.Button(new Rect(Screen.width/2-Screen.width/10, Screen.height/10, Screen.width/5, Screen.height/10), "Send text"))
    // 	{
    // 		bluetoothHelper.SendData(new Byte[] {0, 0, 85, 0, 85});
    //         // bluetoothHelper.SendData("This is a very long long long long text");
    // 	}
    // }
    void OnDestroy()
    {   
        CancelInvoke();
        if (bluetoothHelper != null) bluetoothHelper.Disconnect();
    }
}
