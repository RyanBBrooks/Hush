using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteToParticlesAsset;

public class KeyBehavior : MonoBehaviour
{
    //vars
    SpriteRenderer rend = null; //key sprite
    CircleCollider2D coll = null; //key collider
    ParticleSystem part = null; //particle system
    GameObject target = null; //targeted object
    bool collected = false; //has the key been collected
    DoorBehavior door = null; //door
    LockWallBehavior wall = null; //wall

    //soundvars
    AudioSource src; //source
    public AudioClip collectClip; //key collected clip

    // Start is called before the first frame update
    void Start()
    {
        //get references
        coll = this.GetComponent<CircleCollider2D>();
        rend = this.GetComponent<SpriteRenderer>();
        part = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //if the key has been collected
        if (collected)
        {
            //initialize values
            float speed = 5f;
            float dst = 1.5f;
            Vector2 keyPos = this.gameObject.transform.position;
            Vector2 targetPos = target.transform.position; //target should be lily

            //follow lily
            if ((((Vector3.Distance(keyPos, targetPos)) >= dst) || door || wall))
            {
                this.gameObject.transform.position = Vector2.Lerp(keyPos, targetPos, Time.deltaTime * speed);
            }

            //use key if near door
            if (door && (Vector3.Distance(keyPos, targetPos)) < 0.1)
            {
                delete();
                door.Unlock();
                door = null;
            }
            //use key if near wall
            if (wall && (Vector3.Distance(keyPos, targetPos)) < 0.1)
            {
                delete();
                wall.Break();
                wall = null;
            }
        }
    }

    //collect the key
    public void collect(GameObject t)
    {
        //play collect sound sound
        float collectVol = 1f;
        //src.PlayOneShot(collectClip, collectVol); //CURRENTLY BUGGED

        //change the layer it appears on to allow it to go over objects and appear above fog
        rend.sortingLayerName = "Character";

        
        coll.enabled = false;//disable collider
        target = t;//set target
        collected = true;
        part.Play();//play the particle effect

    }

    //use the key
    public void use(GameObject o)
    {
        //save door and target door
        if (door = o.GetComponent<DoorBehavior>())
        {
            target = o;
        }
        //save wall target wall
        else if (wall = o.GetComponent<LockWallBehavior>())
        {
            target = o;
        }
    }

    //delete the key (visually and functionally)
    public void delete()
    {
        part.Play(); //play particle effect
        rend.enabled = false; //disable visuals
    }

}
