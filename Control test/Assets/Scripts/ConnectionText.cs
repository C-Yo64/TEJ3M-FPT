using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionText : MonoBehaviour
{
    public GameObject serialController;
    GyroTest test;

    public GameObject playButton;

    Text text;

    // Start is called before the first frame update
    void Start()
    {
        serialController = GameObject.Find("SerialController");
        test = serialController.GetComponent<GyroTest>();
        text = gameObject.GetComponent<Text>();

        playButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!text.text.Equals(test.connectionStatus))
        {
            text.text = test.connectionStatus;

            if (text.text.Equals("Not Connected")) {
                text.color = new Color(140, 0, 0, 255);
                text.fontStyle = FontStyle.Bold;
                playButton.SetActive(false);
            }
            if (text.text.Equals("Connected")) {
                text.color = new Color(0, 90, 0, 255);
                text.fontStyle = FontStyle.BoldAndItalic;

                playButton.SetActive(true);
            }
        }
    }
}
