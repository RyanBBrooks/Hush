using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakFloorBehavior : MonoBehaviour
{
    //particle vars
    ParticleSystem part;
    public List<ParticleCollisionEvent> collEvents;
    
    //breaking vars
    public float breakVol = 1.5f;
    public bool detectPlayer = true;
    public bool detectPhys = true;

    //ground vars
    BoxCollider2D box;
    Rigidbody2D rb;
    SpriteRenderer rend;
    private bool broken = false;

    //soundvars
    AudioSource src; //source
    public AudioClip stoneObjectBreakClip; //break clip

    // Start is called before the first frame update
    void Start()
    {
        //get references
        rb = GetComponent<Rigidbody2D>();
        part = GetComponent<ParticleSystem>();
        collEvents = new List<ParticleCollisionEvent>();
        box = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();
        src = GetComponent<AudioSource>(); //for sound
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //get and set if the floor is broken
    public bool GetBroken()
    {
        return broken;
    }
    public void SetBroken(bool b)
    {
        broken = b;
    }

    //break the floor
    public void Break()
    {
        Destroy(rb);
        box.enabled = false;
        rend.enabled = false;

        //TODO : play breaking sound
        PlaySound(breakVol, this.transform.position, stoneObjectBreakClip);

        //play particle animation
        part.Play();
    }

    //when the particles are done, we don't need this anymore
    private void OnParticleSystemStopped()
    {
        Destroy(this.gameObject);
    }

    void OnParticleCollision(GameObject o)
    {
        if (collEvents != null)
        {
            //get all collisions
            int n = part.GetCollisionEvents(o, collEvents);
            int i = 0;

            //iterate through, spawn echo circle
            for (i = 0; i < n; i++)
            {
                Vector3 pos = collEvents[i].intersection; //collision position
                float vol = collEvents[i].velocity.magnitude / 70; //collision volume

                //if too quiet don't play sound, otherwise do
                if (vol > 0.1)
                {
                    //spawn a "EchoCircle"
                    CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
                    s.SpawnEchoCircleInExtents(pos, vol);
                }
            }
        }
    }

    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //ignore broken clips
        if (!clip) return;

        //play sound
        src.PlayOneShot(clip, vol);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircleInExtents(pos, vol);
    }
}
