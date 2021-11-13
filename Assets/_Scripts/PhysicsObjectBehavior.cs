using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObjectBehavior : MonoBehaviour
{
    //visual vars
    public GameObject sprite; // represtents the visual layer
    SpriteRenderer spriteRend; // the renderer for the sprite
    bool visible = false; // is the sprite currently visible

    //physical vars
    GameObject box; // represents the physical layer

    //rope vars
    public List<GameObject> ropes = new List<GameObject>();
    public bool initialized = false;

    //grab vars
    public bool isGrabbable = false;
    public bool grabLocksRotation = false; //wheter or not grabbing the object locks its rotation (good for things that roll)
    DistanceJoint2D joint;

    //stasis animation vars
    public float alphaMin = 0.6f; // the minimum alpha reached while the object is "lost"
    public float alphaDecayRate = 0.15f; //the decay rate of the alpha

    //audio vars
    //SOUND: Create AudioClip variable for each different sound category in a prefab    
    public AudioClip thudClip;
    //SOUND: Create one AudioSource variable for audio source
    AudioSource src;


    // Start is called before the first frame update
    void Start()
    {
        //update refrences
        box = this.gameObject;
        spriteRend = sprite.GetComponent<SpriteRenderer>();
        joint = this.gameObject.GetComponent<DistanceJoint2D>();

        //SOUND: get reference for each audio sorce in a prefab
        src = GetComponent<AudioSource>();

        //initialize values of the joint (just in case)
        joint.enabled = false;
        joint.distance = 0;

        //set the sprite to transparent to start
        spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 0);


    }

    //returns if the object is set to grabbable
    public bool GetIsGrabbable()
    {
        return isGrabbable;
    }

    //returns if grabbing the object should lock it's rotation
    public bool GetGrabLocksRotation()
    {
        return grabLocksRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //initialize ropes
        if (!initialized)
        {
            initialized = true;
            //update ropes
            foreach (GameObject rope in ropes)
            {
                Rope s = rope.GetComponent<Rope>();
                s.ChangeVisualColorAlpha(0);
            }
        }
        //if an object is visible, decrease it's alpha until it reaches the minimum set by alphaMin
        if (!visible && spriteRend.color.a > alphaMin)
        {
            float newA = spriteRend.color.a - (alphaDecayRate * Time.deltaTime);
            spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, newA);
            foreach (GameObject rope in ropes)
            {
                Rope s = rope.GetComponent<Rope>();
                s.ChangeVisualColorAlpha(newA);
            }
        }
    }

    //updates visual position etc of sprite
    public void UpdateVisual()
    {
        visible = true;
        spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 1);
        sprite.transform.position = box.transform.position;
        sprite.transform.rotation = box.transform.rotation;
        foreach (GameObject rope in ropes)
        {
            Rope s = rope.GetComponent<Rope>();
            s.UpdateVisual();
            s.ChangeVisualColorAlpha(1);
        }
    }

    //update whether the object is visible (externally)
    public void SetVisible(bool _visible)
    {
        visible = _visible;
    }

    //if we collide with something
    private void OnCollisionEnter2D(Collision2D col)
    {
        //TODO: update this - for now, don't make sounds if the player hits an object, should probbably be handled by player
        //if (col.gameObject.tag == "Player")
        //{
            //return;
        //}

        //TODO: perfect impact based volume calculation
        Vector2 pos = Vector2.zero; //position for the sound to be played
        Vector2 avgNormal = Vector2.zero; //average Normal vector for the contacts

        //calculate avgNormal and pos
        foreach (ContactPoint2D contact in col.contacts)
        {
            pos += contact.point;
            avgNormal += contact.normal;
        }
        avgNormal /= col.contactCount; //divide by num of contacts

        //calculate the volume based on the "intensity" of the impact
        Rigidbody2D other = col.gameObject.GetComponent<Rigidbody2D>();
        float vol = Vector2.Dot(avgNormal, col.relativeVelocity) / 10;

        //only play a sound if the volume is above a certain threshold ----- POSSIBLY CHANGE THIS TO JUST EFFECT VISUALS LATER
        if (vol > 0.2f)
        {
            PlaySound(vol, pos / col.contactCount, thudClip);
        }
    }

    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //UNCOMMENT ME ONCE CLIP EXISTS
        //src.PlayOneShot(clip, vol);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircle(pos, vol);
    }
}
