using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gets the variables from the gyro reading script and moves the ball based on the direction
public class Player : MonoBehaviour
{
    // Reference to the object that communicates with the controller/stores the gyro readings
    public GameObject serialController;
    // Reference to the script attached to the object that contains the variables for the gyro and capacitor
    public GyroTest gyro;

    // Reference to the physics component of the sphere (used for adding force based on input directions)
    public Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        // Automatically find the objects/components that these variable reference
        serialController = GameObject.Find("SerialController");
        gyro = serialController.GetComponent<GyroTest>();
    }

    // Update is called once per frame
    void Update()
    {
        // Add force in the direction that the gyro is tilting
        // The force is multiplied by the charge of the capacitor, so the more charge it has, the faster you go
        // X had to be inverted to stop it from being reversed, and the capacitor charge had to be divided by a large number since the range can go from 0-4095
        rigidbody.AddForce(new Vector3((-gyro.X * 1.3f) * (1 + (gyro.capCharge / 2500f)), 0, (gyro.Y * 1.3f) * (1 + (gyro.capCharge / 2500f))));
    }
}
