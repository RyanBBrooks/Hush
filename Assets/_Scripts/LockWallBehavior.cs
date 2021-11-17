using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    //break the wall
    public void Break()
    {
        box.enabled = false;
        rend.enabled = false;
        //destroy detector
        Destroy(this.gameObject.transform.GetChild(0).gameObject);
        //TODO : play breaking sound
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircle(this.transform.position, breakVol);

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
        //get all collisions
        int n = part.GetCollisionEvents(o, collEvents);
        int i = 0;

        //iterate through, spawn echo circle
        for (i=0;i<n;i++)
        {
            Vector3 pos = collEvents[i].intersection;
            float vol = collEvents[i].velocity.magnitude / 70;
            if (vol > 0.1)
            {
                PlaySound( vol, pos, stoneObjectBreakClip);
            }
        }
    }

    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //UNCOMMENT ME ONCE CLIP EXISTS
        src.PlayOneShot(clip, vol);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircle(pos, vol);
    }
}
