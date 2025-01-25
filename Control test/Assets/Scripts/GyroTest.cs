using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

// Takes the message sent from the controller and gets the gyro/capacitor values from it
public class GyroTest : MonoBehaviour
{
    // Reference to the object that communicates with the controller
    public SerialController serialController;

    // Stores the gyro readings and the capacitor's charge
    public float X, Y, Z;
    public float capCharge;

    // The string that gets displayed on the menu showing if the controller is connected
    public string connectionStatus = "Not Connected";

    // Stores the currently running instance of this script so that only one instance runs at a time
    private static GyroTest instance;

    void Awake()
    {
        if (instance == null)
        {
            // If no instances already exist, set this as the instance
            instance = this;
            DontDestroyOnLoad(gameObject); // Don't remove this object when I load another scene
        }
        else if (instance != this)
        {
            // If an instance already exists, disable this one
            Debug.LogWarning("Duplicate SerialController detected. Disabling this instance.");
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // If the player clicks the mouse, send a message that will recenter the gyro when the ESP receives it
        if (Input.GetButtonDown("Fire1"))
        {
            serialController.SendSerialMessage("1");
        }
    }

    // Runs when the ESP sends a message
    public void OnMessageArrived(string message)
    {
        // Check if the message matches what would contain the gyro values
        if (message.Contains("X: ") && message.Contains("Y: ") && message.Contains("Z: "))
        {
            // Store the gyro readings and charge as strings which will later be converted to floats
            string x = "";
            string y = "";
            string z = "";

            string charge = "";

            // The counters are used to keep track of when each reading will start in the message
            int yCounter = 0;
            bool yCounterStart = false;
            int zCounter = 0;
            bool zCounterStart = false;

            int chargeCounter = 0;
            bool chargeCounterStart = false;

            // Loop through each character in the messgae
            for (int i = 0; i < message.Length; i++)
            {
                // The message starts with the X reading, so count up until the numbers begin and end after they stop
                // X
                if (i >= 3 && i <= 7)
                {
                    // Make sure it doesn't go over the numbers and include non-number characters in the string
                    if (!message[i].Equals(','))
                    {
                        // Add the current character to the string storing the X value
                        x += message[i];
                    }
                }

                // If the character is 'Y', start counting until the Y value starts
                // Y
                if (message[i].Equals('Y'))
                {
                    yCounterStart = true;
                }
                if (yCounterStart)
                {
                    // Once counting, store the digits the same way the X value is done
                    yCounter++;
                    if (yCounter >= 3 && yCounter <= 7)
                    {
                        if (!message[i].Equals(','))
                        {
                            y += message[i];
                        }
                        else // Stop the counter when the value is done to prevent non-number characters in the string
                        {
                            yCounterStart = false;
                        }
                    }
                }

                // If the character is 'Z', start counting until the Z value starts
                // Z
                if (message[i].Equals('Z'))
                {
                    zCounterStart = true;
                }
                if (zCounterStart)
                {
                    // Once counting, store the digits the same way the X value is done
                    zCounter++;
                    if (zCounter >= 3 && zCounter <= 7)
                    {
                        if (!message[i].Equals(','))
                        {
                            z += message[i];
                        }
                        else // Stop the counter when the value is done to prevent non-number characters in the string
                        {
                            zCounterStart = false;
                        }
                    }
                }

                // If the character is 'C', start counting until the capactior's charge value starts
                if (message[i].Equals('C'))
                {
                    chargeCounterStart = true;
                }
                if (chargeCounterStart)
                {
                    // Once counting, store the digits the same way the X value is done, but accounting for the extra characters in the word "Charge"
                    chargeCounter++;
                    if (chargeCounter >= 8 /*&& chargeCounter <= 7*/)
                    {
                        if (!message[i].Equals(','))
                        {
                            charge += message[i];
                        }
                        else // Stop the counter when the value is done to prevent non-number characters in the string
                        {
                            chargeCounterStart = false;
                        }
                    }
                }


            }

            // Convert the contents of the strings to floats and print them to the console

            print("X: " + x);
            X = Single.Parse(x);

            print("Y: " + y);
            Y = Single.Parse(y);

            print("Z: " + z);
            Z = Single.Parse(z);

            print("Charge: " + charge);
            capCharge = Single.Parse(charge);
        }

    }

    // Runs when the ESP is either connected or disconnected
    public void OnConnectionEvent(bool connection)
    {
        // If it's been connected, set the connection status to "Connected", otherwise set it to "Not Connected"
        switch (connection)
        {
            case true:
                connectionStatus = "Connected";
                break;
            case false:
                connectionStatus = "Not Connected";
                break;
        }
    }

    // public void EnableSerialController()
    // {
    //     gameObject.SetActive(true);
    // }

}
