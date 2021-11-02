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
    Animator anim = null;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("IsLocked", locked);
    }

    //lock aor unlock door
    internal void SetDoorLocked(bool l)
    {
        locked = l;
    }

    //begin animation for door transition
    internal void BeginSceneTransition()
    {
        //play - animate the door closing -> this automatically triggers changeScene()
        anim.SetBool("ExitLevel", true);       
    }

    //change scene called by door animator
    internal void ChangeScene()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }  
}
