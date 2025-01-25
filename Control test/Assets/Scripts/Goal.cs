using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Loads the scene specified
public class Goal : MonoBehaviour
{
    // The scene to load
    public string scene;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // If the player collides with the goal flag, load the scene specified (always set to the menu scene)
    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag.Equals("Player"))
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
    }
}
