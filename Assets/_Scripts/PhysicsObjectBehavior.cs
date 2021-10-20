using UnityEngine;

public class PhysicsObjectBehavior : MonoBehaviour
{
    //visual vars
    public GameObject sprite; // represtents the visual layer
    SpriteRenderer spriteRend; // the renderer for the sprite
    bool visible = false; // is the sprite currently visible

    //physical vars
    GameObject box; // represents the physical layer

    //grab vars
    public bool isGrabbable = false;
    DistanceJoint2D joint;
    
    //stasis animation vars
    public float alphaMin = 0.6f; // the minimum alpha reached while the object is "lost"
    public float alphaDecayRate = 0.15f; //the decay rate of the alpha

    // Start is called before the first frame update
    void Start()
    {
        //update refrences
        box = this.gameObject;
        spriteRend = sprite.GetComponent<SpriteRenderer>();
        joint = this.gameObject.GetComponent<DistanceJoint2D>();

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

    // Update is called once per frame
    void Update()
    {
        //if an object is visible, decrease it's alpha until it reaches the minimum set by alphaMin
        if(!visible && spriteRend.color.a > alphaMin)
        {
            spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, spriteRend.color.a - (alphaDecayRate * Time.deltaTime));
        }
    }

    //updates visual position etc of sprite
    public void UpdateVisual()
    {
            visible = true;
            spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 1);
            sprite.transform.position = box.transform.position;
            sprite.transform.rotation = box.transform.rotation;
    }

    //update whether the object is visible (externally)
    public void SetVisible(bool _visible)
    {
        visible = _visible;
    }

    //if we collide with something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO: perfect impact based volume calculation
        Vector2 pos = Vector2.zero; //position for the sound to be played
        Vector2 avgNormal = Vector2.zero; //average Normal vector for the contacts

        //calculate avgNormal and pos
        foreach(ContactPoint2D contact in collision.contacts)
        {
            pos += contact.point;
            avgNormal += contact.normal;
        }
        avgNormal /= collision.contactCount; //divide by num of contacts

        //calculate the volume based on the "intensity" of the impact
        Rigidbody2D other = collision.gameObject.GetComponent<Rigidbody2D>();
        float vol = Vector2.Dot(avgNormal, collision.relativeVelocity) / 10;

        //only play a sound if the volume is above a certain threshold ----- POSSIBLY CHANGE THIS TO JUST EFFECT VISUALS LATER
        if (vol > 0.3f)
        {
            PlaySound(vol, pos / collision.contactCount);
        }
    }

    public void PlaySound(float vol, Vector2 pos /**AudioClip sound**/)
    {
        //TODO: Play actoual audio clip, sound here

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircle(pos, vol);
    }
}