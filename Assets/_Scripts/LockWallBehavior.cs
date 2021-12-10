using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class LockWallBehavior : MonoBehaviour
{
    //particle vars
    ParticleSystem part;
    public List<ParticleCollisionEvent> collEvents;
    bool targeted = false;

    //breaking vars
    public float breakVol = 1.5f;

    //ground vars
    BoxCollider2D box;
    SpriteRenderer rend;

    //soundvars
    AudioSource src; //source
    public AudioClip stoneObjectBreakClip; //break clip

    // Start is called before the first frame update
    void Start()
    {
        //get references
        part = GetComponent<ParticleSystem>();
        collEvents = new List<ParticleCollisionEvent>();
        box = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();

        //SOUND: get audio sorce
        src = GetComponent<AudioSource>();
    }

    //set that the wall is currently being targeted by a key
    public void setTargeted(bool t)
    {
        targeted = t;
    }
    //return if the wall is currently targeted by a key
    public bool getTargeted()
    {
        return targeted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //break the wall
    public void Break()
    {
        //disable the renderer and the collision box
        box.enabled = false;
        rend.enabled = false;

        //destroy the detector
        Destroy(this.gameObject.transform.GetChild(0).gameObject);

        //play breaking sound
        PlaySound(breakVol, this.transform.position, stoneObjectBreakClip);

        //play particle animation
        part.Play();
    }

    //when the particles are done, we don't need this anymore
    private void OnParticleSystemStopped()
    {
        Destroy(this.gameObject);
    }
    
    //spawn echo circles when particle collide
    void OnParticleCollision(GameObject o)
    {
        //get all collisions
        int n; 
        int i = 0;

        //ensure collision events is not null.
        try
        {
            //get the collision events
            n = part.GetCollisionEvents(o, collEvents);

            //iterate through, spawn echo circles for each colliding particle
            for (i = 0; i < n; i++)
            {
                Vector3 pos = collEvents[i].intersection; //collision position
                float vol = collEvents[i].velocity.magnitude / 70; //volume of the collision

                //if we have a loud enough collision display the echo circle
                if (vol > 0.1)
                {
                    //spawn a "EchoCircle"
                    CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
                    s.SpawnEchoCircleInExtents(pos, vol);
                }
            }
        }
        //otherwise ignore particles as a failsafe
        catch (ArgumentNullException)
        {
            return;
        }
    }

    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //if the clip is missing return
        if (!clip) return;

        //otherwise play a sound
        src.PlayOneShot(clip, vol);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircleInExtents(pos, vol);
    }
}
