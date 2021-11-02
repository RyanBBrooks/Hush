using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorBehavior : MonoBehaviour
{
    //vars
    public bool locked = false; // if the door is locked
    public string sceneName = "PhysicsTest"; //scene to load on door open

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void UnlockDoor()
    {
        locked = false;
    }

    internal void BeginSceneTransition()
    {
        //TODO: play - animate the door closing !!!!!!!!!!!!!!!make sure this animation has a few seconds on the open door to allow character to fade!!!!!!!!!!!!!!!!

        //load attached scene <--- TODO: move this to be triggered by animation finished
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
