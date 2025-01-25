using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays text showing the connection status to the controller, and turns on/off the play button based on it
public class ConnectionText : MonoBehaviour
{
    // Reference to the object that communicates with the controller/stores the gyro readings
    public GameObject serialController;
    // Reference to the script attached to the object that contains the variables for the gyro and capacitor
    GyroTest test;

    // Reference to the button on the menu
    public GameObject playButton;

    // Reference to the text that will be displayed
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        // Automatically find the objects/components that these variable reference
        serialController = GameObject.Find("SerialController");
        test = serialController.GetComponent<GyroTest>();
        text = gameObject.GetComponent<Text>();

        // Turn off the play button until the controller is connected
        playButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the connection status has changed
        if (!text.text.Equals(test.connectionStatus))
        {
            // Update the displayed text based on the connection status
            text.text = test.connectionStatus;

            // If it's not connected, disable the play button and set the text style to red/bold
            if (text.text.Equals("Not Connected")) {
                text.color = new Color(140, 0, 0, 255);
                text.fontStyle = FontStyle.Bold;
                playButton.SetActive(false);
            }
            // If it's connected, enable the play button and set the text style to green/bold and italic
            if (text.text.Equals("Connected")) {
                text.color = new Color(0, 90, 0, 255);
                text.fontStyle = FontStyle.BoldAndItalic;

                playButton.SetActive(true);
            }
        }
    }
}
