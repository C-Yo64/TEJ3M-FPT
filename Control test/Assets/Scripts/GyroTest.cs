using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GyroTest : MonoBehaviour
{
    public SerialController serialController;

    public float X, Y, Z;
    public float capCharge;

    public string connectionStatus = "Not Connected";

    private static GyroTest instance; // Singleton instance

    void Awake()
    {
        if (instance == null)
        {
            // If no instance exists, set this as the instance
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else if (instance != this)
        {
            // If an instance already exists, disable or destroy this one
            Debug.LogWarning("Duplicate SerialController detected. Disabling this instance.");
            gameObject.SetActive(false); // Or Destroy(gameObject) if preferred
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
        if (Input.GetButtonDown("Fire1"))
        {
            serialController.SendSerialMessage("1");
        }
    }

    public void OnMessageArrived(string message)
    {
        if (message.Contains("X: ") && message.Contains("Y: ") && message.Contains("Z: "))
        {
            string x = "";
            string y = "";
            string z = "";

            string charge = "";

            int yCounter = 0;
            bool yCounterStart = false;
            int zCounter = 0;
            bool zCounterStart = false;

            int chargeCounter = 0;
            bool chargeCounterStart = false;

            for (int i = 0; i < message.Length; i++)
            {
                // X
                if (i >= 3 && i <= 7)
                {
                    if (!message[i].Equals(','))
                    {
                        x += message[i];
                    }
                }

                // Y
                if (message[i].Equals('Y'))
                {
                    yCounterStart = true;
                }
                if (yCounterStart)
                {
                    yCounter++;
                    if (yCounter >= 3 && yCounter <= 7)
                    {
                        if (!message[i].Equals(','))
                        {
                            y += message[i];
                        }
                        else
                        {
                            yCounterStart = false;
                        }
                    }
                }

                // Z
                if (message[i].Equals('Z'))
                {
                    zCounterStart = true;
                }
                if (zCounterStart)
                {
                    zCounter++;
                    if (zCounter >= 3 && zCounter <= 7)
                    {
                        if (!message[i].Equals(','))
                        {
                            z += message[i];
                        }
                        else
                        {
                            zCounterStart = false;
                        }
                    }
                }

                if (message[i].Equals('C'))
                {
                    chargeCounterStart = true;
                }
                if (chargeCounterStart)
                {
                    chargeCounter++;
                    if (chargeCounter >= 8 /*&& chargeCounter <= 7*/)
                    {
                        if (!message[i].Equals(','))
                        {
                            charge += message[i];
                        }
                        else
                        {
                            chargeCounterStart = false;
                        }
                    }
                }


            }

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

    public void OnConnectionEvent(bool connection)
    {
        switch (connection)
        {
            case true:
                connectionStatus = "Connected";
                break;
            case false:
                connectionStatus = "Not Connected";
                // if (!SceneManager.GetActiveScene().Equals("Menu"))
                // {
                //     SceneManager.LoadScene("Menu");
                // }
                // Invoke("EnableSerialController", 1f);
                // gameObject.SetActive(false);
                break;
        }
    }

    public void EnableSerialController()
    {
        gameObject.SetActive(true);
    }

}
