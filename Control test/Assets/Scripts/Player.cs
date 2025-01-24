using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject serialController;
    public GyroTest test;

    public Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        serialController = GameObject.Find("SerialController");
        test = serialController.GetComponent<GyroTest>();


        // Physics.gravity = new Vector3(0, -9.81f, 0);
        // Time.timeScale = 1f;
        // Rigidbody rb = FindObjectOfType<Rigidbody>();
        // if (rb != null)
        // {
        //     rb.velocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.AddForce(new Vector3((-test.X * 1.3f)*(1+(test.capCharge/2500f)), 0, (test.Y * 1.3f)*(1+(test.capCharge/2500f))));
    }
}
