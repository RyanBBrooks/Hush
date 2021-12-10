using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorBehavior : MonoBehaviour
{
    //vars
    public bool locked = false; // if the door is locked
    public string sceneName = "Level0_TitleScene"; //default scene to load on door open
    Animator anim = null;
    bool targeted = false; //is the door targeted by a key

   
    //soundvars
    AudioSource src; //source
    public AudioClip openUnlockClip; //key collected clip
    //we shouldnt actually be closing doors so we dont really need a close clip, the functionality was added as a backup

    // Start is called before the first frame update
    void Start()
    {
        //get references
        anim = GetComponent<Animator>();
        src = GetComponent<AudioSource>();
    }

    //sets / gets targeted bool to let door know if a key is on the way!
    public void setTargeted(bool t)
    {
        targeted = t;
    }
    public bool getTargeted()
    {
        return targeted;
    }

    // Update is called once per frame
    void Update()
    {
        //animate based on current value of locked
        anim.SetBool("IsLocked", locked);
    }

    //unlock the door
    public void Unlock()
    {
        //unlock door
        locked = false;

        //Play unlock + open sound (NO VISUALILZATION)
        float openUnlockVol = 3f;
        src.PlayOneShot(openUnlockClip, openUnlockVol);

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
        //load the new scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }  
}
