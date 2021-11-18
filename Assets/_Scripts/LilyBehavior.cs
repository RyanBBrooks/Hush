using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Ryan Brooks u1115093
public class LilyBehavior : MonoBehaviour
{

    //movement vars
    public float max_speed = 7.5F; //the max speed of the player
    public float smoothing = 0.01f; //movement smoothing
    public float jumpForce = 500f; //the jump impulse of the player
    public float antiFloatForce = -2.5f; // force acting against the player when not holding space (helps control jump height)
    private Vector3 vel = Vector3.zero;

    Rigidbody2D body; // the player's body
    int groundLayer = 7; //layer mask referring to the ground layer (layer #7 should be)
    int physLayer = 13; //layer mask referring to the physics layer (layer #13 should be)
    bool flipX = false; // is the player's x flipped, used for ray direction
    bool isOnGround = false; // is the player on the ground currently
    bool isJumping = false;
    PolygonCollider2D hitBox;
    BoxCollider2D feetBox;

    //grab/push/pull vars
    public float grabStartDist = 0.75f; // the distance from which the player can grab objects
    public LayerMask pushRayMask; // a layermask that prevents the player's ray from hitting themselves
    public float grabSpeedMult = 0.75f; // multiplied by speed when grabbing
    public float jointDistMod = 0.2f; // modifier for pull (joint) distance

    GameObject grabbedObject; // the object the player is currently grabbing
    bool isAttachedGrab = false; //is the player currently attached to the GameObject grabbed
    bool isTryMove = false; //is the player trying to input movement
    bool isWalkPushing = false; //is the player pushing something by walking

    //animation vars
    SpriteRenderer sprite; // the player's sprite
    Animator spriteAnim; // the player's animator
    public bool isStepping = false; //from the animator, if the player is stepping
    bool hasStepped = false;

    //door vars
    bool isTransitioning = false; //are we doing a scene transition
    List<GameObject> keys = null; //current number of keys held by lily

    //Audio Reveal vars
    public float minReveal = 0.3f;
    public float maxReveal = 2f;
    float revealTimer = 0f;
    public float revealVolMult = 1.2f;
    bool isChargingReveal = false;
    ParticleSystem revealParticles = null;

    //death vars
    bool dead = false;


    //sound vars
    public float stepVol = 0.25f; // the volume of a step
    //SOUND: Create one AudioSource variable for audio source
    AudioSource src;
    //SOUND: Create sound for footstep
    public AudioClip footstepClip;
    public AudioClip revealClip; //used to be clap is now "flute??"
    public AudioClip fallClip; //hitting the ground
    public AudioClip deathClip;

    public GameObject monster; //NOTE: not really sure why these are here?
                               //shouldnt they be in the monster's script?
    public Animator monsterAnimate;

    // Start is called before the first frame update
    void Start()
    {
        //get components to initialize them
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponent<PolygonCollider2D>();
        feetBox = GetComponent<BoxCollider2D>();

        gameObject.GetComponent<SpriteRenderer>().color = new Color(255,255,255);
        spriteAnim = GetComponent<Animator>();
        keys = new List<GameObject>();
        revealParticles = GetComponent<ParticleSystem>();

        //SOUND: get audio sorce
        src = GetComponent<AudioSource>();
    }

    //Determine if lily is on the ground by checking the collision box
    private void OnTriggerExit2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.layer == groundLayer || o.layer == physLayer)
        {
            isOnGround = false;
            spriteAnim.SetBool("isFalling", true);
        }

        
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (dead) return;
        //if it is the ground
        GameObject o = col.gameObject;
        if (o.layer == groundLayer || o.layer == physLayer)
        {
            spriteAnim.SetBool("isFalling", false);
            isOnGround = true;
            if (isJumping)
            {
                isJumping = body.velocity.y > 0.05; //update jumping to be false if are hitting the ground
            }
        }
        //if we are in front of a door trigger
        if (o.tag == "Door" && !isChargingReveal)
        {
            //get the door script
            DoorBehavior s = o.GetComponent<DoorBehavior>();
            //if we press up and are on the ground
            if (Input.GetKey(KeyCode.W) && isOnGround)
            {
                //if the door is locked check if we have a key, use the key
                if (s.locked && keys.Count>0 && !s.getTargeted())
                {
                    KeyBehavior k = keys[keys.Count-1].GetComponent<KeyBehavior>();
                    s.setTargeted(true);
                    k.use(o);
                    keys.RemoveAt(keys.Count-1);                  
                }
                //otherwise if the door is unlocked, go through the door
                else if (!s.locked)
                {
                    //set the player to not be able to move/interact
                    body.simulated = false;
                    hitBox.enabled = false;
                    feetBox.enabled = false;
                    isTransitioning = true;
                    //delete all keys
                    foreach (GameObject key in keys)
                    {
                        KeyBehavior k = key.GetComponent<KeyBehavior>();
                        k.delete();
                    }
                    keys.Clear();
                    //animate walking
                    spriteAnim.SetFloat("Speed", 1);
                    //load a new scene from door script
                    s.BeginSceneTransition();
                }
            }
        }
        //if we are in front of a key wall trigger
        if (o.tag == "KeyWall" && !isChargingReveal)
        {
            //get the key wall (checks too see if it is broken)
            GameObject wall = o.GetComponent<LockWallDetectorBehavior>().getWall();
            LockWallBehavior s = wall.GetComponent<LockWallBehavior>();
            //if we get a valid key wall              
            //if we have a key, use the key (wall isnt being unlocked "targeted")
            if (keys.Count > 0 && !s.getTargeted())
            {
                KeyBehavior k = keys[keys.Count - 1].GetComponent<KeyBehavior>();
                s.setTargeted(true);
                k.use(wall);
                keys.RemoveAt(keys.Count - 1);
            }
        }

        if (o.tag == "Death")
        {
            Die();
        }

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (dead) return;
        GameObject o = col.gameObject;
        //if we come into contact with a key
        if (o.tag == "Key" && !keys.Contains(o))
        {
            KeyBehavior s = o.GetComponent<KeyBehavior>();
            //calculate target
            int n = keys.Count;
            GameObject t = this.gameObject;
            if (n > 0)
            {
                t = keys[n - 1];
            }
            s.collect(t);
            keys.Add(o);
        }
        else if (o.tag == "MonsterTrigger")
        {
            monster.SetActive(true);
        }
        else if(o.tag == "MonsterAnim")
        {
            monsterAnimate.SetFloat("Speed", 2);
        }
    }

    //play hit ground sound when we land on something from over a certain distance
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == groundLayer)
        {
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
            float vol = Vector2.Dot(avgNormal, col.relativeVelocity) / 15;

            //only play a sound if the volume is above a certain threshold 
            if (vol > 0.2f)
            {
                PlaySound(vol, pos / col.contactCount, fallClip);
            }
        }
    }

    //When lily stops touching a physics object
    private void OnCollisionExit2D(Collision2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            PhysicsObjectBehavior s = o.gameObject.GetComponent<PhysicsObjectBehavior>();
            s.SetVisible(false);

            //stop possible (non grab) pushing animation, because we are no longer colliding
            if (!isAttachedGrab)
            {
                spriteAnim.SetBool("isGrabbing", false);
            }
            isWalkPushing = false;
        }
        
    }

    //When lily touches a physics object
    private void OnCollisionStay2D(Collision2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            PhysicsObjectBehavior s = o.gameObject.GetComponent<PhysicsObjectBehavior>();
            s.UpdateVisual();    

            //if we are not attaced to an object, check if we are pushing it
            if (!isAttachedGrab)
            {
                //calculate the average normal vector to the collision
                Vector2 avgNormal = Vector2.zero;
                foreach (ContactPoint2D cont in col.contacts)
                {
                    avgNormal += cont.normal;
                }
                avgNormal /= col.contactCount; //average out the values

                // we are in horizontal contact if the normal is close to horizontal           
                bool inHorizContact = Mathf.Abs(avgNormal.y) < 0.2;

                //if we are in horizontal contact, on the ground, and trying to move
                if (inHorizContact && isOnGround && isTryMove)
                {
                    //update the pushing animation to be on
                    spriteAnim.SetBool("isGrabbing", true);
                    isWalkPushing = true;
                }
                else
                {
                    //stop any possible pushing animations, we have broke horizontal, grounded contact
                    isWalkPushing = false;
                    if (!isAttachedGrab)
                    {
                        spriteAnim.SetBool("isGrabbing", false);
                    }
                }
            }
        }
    }

    //kill lily
    public void Die()
    {
        if (dead) return;
        //set player to be dead
        dead = true;
        // play animation -> load scene
        spriteAnim.SetBool("isDead", true);

        //delete all keys
        foreach (GameObject key in keys)
        {
            KeyBehavior k = key.GetComponent<KeyBehavior>();
            k.delete();
        }
        keys.Clear();

        //stop any particles
        revealParticles.Stop();

        PlaySound(0.5f, transform.position, deathClip);
    }

    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //UNCOMMENT ME ONCE CLIP EXISTS
        src.PlayOneShot(clip, vol*6);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircle(pos, vol);
    }

    void Update()
    {
        //Debug.Log(spriteAnim.GetBool("isGrabbing"));
        if (dead)
        {           
            //just slow down character motion
            Vector2 target = new Vector2(0, -5);
            body.velocity = Vector3.SmoothDamp(body.velocity, target, ref vel, smoothing);
            return;
        }
        if (!isTransitioning)
        {

            //Horizontal Movement
            float x = Input.GetAxis("Horizontal");

                //Prevent Undermoving by Lower Bounding Axis Values
                if (Mathf.Abs(x) < 0.2)
                {
                    x = 0;
                    isTryMove = false;
                }
                else
                {
                    isTryMove = true;
                }

            //if we are pulling something, cut speed
            float m_speed = max_speed;
            if (isAttachedGrab || isWalkPushing)
            {
                m_speed *= grabSpeedMult;
            }
            //lock movement if charging
            if (isChargingReveal)
            {
                m_speed = 0;
            }


            //Update Velocity
            float finalSpeed = x * m_speed;
            Vector2 target = new Vector2(finalSpeed, body.velocity.y);
            body.velocity = Vector3.SmoothDamp(body.velocity, target, ref vel, smoothing);

            //update speed to induce movement animation
            spriteAnim.SetFloat("Speed", Mathf.Abs(finalSpeed));

            //TODO: switch animations if we are pushing and pulling (WHILE GRABBED)

            //flip sprite according to movement direction
            if (!isAttachedGrab)
            {
                if (x < 0 && body.velocity.x < 0)
                {
                    sprite.flipX = flipX = true;
                }
                if (x > 0 && body.velocity.x > 0)
                {
                    sprite.flipX = flipX = false;
                }

            }
            //calculate pushing animation
            spriteAnim.SetFloat("PushDir", finalSpeed*(flipX? -1 : 1));

            //Footstep EchoCircles & sounds
            if (isOnGround && isStepping && !hasStepped)
            {
                //set circle position at foot and slightly in front of character to account for movement speeed
                Vector2 stepLocation = new Vector2(this.gameObject.transform.position.x + body.velocity.x / 20,
                                                    this.gameObject.transform.position.y - this.gameObject.transform.localScale.y); ;

                //TODO : Play a footstep sound here!
                PlaySound(stepVol, stepLocation, footstepClip);

                //reset vars
                isStepping = false; // in case this is the last tick of the frame
                hasStepped = true;
            }
            // a check since the step is updated every tick while animating
            else if (!isStepping)
            {
                hasStepped = false;
            }

            //prevent jumping if we are clapping / charging
            //prevent jumping if we are grabbing, if we press the jump button down, and are on ground and are not jumping
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                && !isAttachedGrab && isOnGround && !isJumping && !isChargingReveal)
            {
                isOnGround = false; //gaurentee we are off the ground once we start jumping.
                //isJumping = true;
                spriteAnim.SetTrigger("jump");

                body.velocity = new Vector2(body.velocity.x, 0);//NEW set velocity to zero before jump in y dir to avoid high jumps

                body.AddForce(new Vector2(0f, jumpForce)); // jump
            }
            //Holding space increases float (we do this by adding a negative force while jumping and not holding space)
            else if (!(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0)) &&
                !isOnGround)
            {
                body.AddForce(new Vector2(0f, antiFloatForce)); //downward force applied
            }

            //Begin Grabbing Code
            //if we are holding the grab key, and we are not currently grabbing, and we are on the ground, not clapping...
            if (Input.GetKey(KeyCode.LeftShift) && !isAttachedGrab && isOnGround && !isChargingReveal)
            {
                //cast a ray to check if there is a grabbable object
                Physics2D.queriesStartInColliders = false;
                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, (flipX ? Vector2.left : Vector2.right) * transform.localScale.x, grabStartDist, pushRayMask);

                //if we hit an object that is tagged phys
                if (hit.collider != null && ((grabbedObject = hit.collider.gameObject).tag == "Phys"))
                {
                    DistanceJoint2D j;
                    PhysicsObjectBehavior s = grabbedObject.gameObject.GetComponent<PhysicsObjectBehavior>();

                    //if the object is grabbable and has a DistanceJoint
                    if (s.GetIsGrabbable() && (j = grabbedObject.GetComponent<DistanceJoint2D>()) != null)
                    {
                        j.enabled = isAttachedGrab = true; //enable the joint, we are not attached
                        j.connectedBody = body; //it is connected to lily's body
                        spriteAnim.SetBool("isGrabbing", true);

                        //if object's rotation should be locked, do it
                        grabbedObject.gameObject.GetComponent<Rigidbody2D>().freezeRotation = s.GetGrabLocksRotation();

                        //set the anchor to the position where our ray hit the object relative to the origin of that object
                        j.anchor = Quaternion.Inverse(grabbedObject.transform.rotation) * ((Vector3)hit.point - grabbedObject.transform.position);

                        //set the distance to the difference between lily's position and the hit point + some modifier
                        j.distance = (hit.point - body.position).magnitude + jointDistMod;

                        Debug.DrawLine(hit.point, body.position, Color.red, 2.5f, false); //<--- draws a line to test the drag distance

                        isWalkPushing = false; //not push walking as we are grabbing
                    }
                }
                Physics2D.queriesStartInColliders = true; //reset physics calculations to be thurough            
            }

            //StopGrabbing/WhileGrabbing Code
            if (isAttachedGrab)
            {
                PhysicsObjectBehavior s = grabbedObject.gameObject.GetComponent<PhysicsObjectBehavior>();

                //if we should stop grabbing
                if (!isOnGround || !Input.GetKey(KeyCode.LeftShift))
                {
                    //find the joint, disable it.
                    DistanceJoint2D j;
                    if ((j = grabbedObject.GetComponent<DistanceJoint2D>()) != null)
                    {
                        j.enabled = isAttachedGrab = false;
                        s.SetVisible(false); //begin fade
                        spriteAnim.SetBool("isGrabbing", false);
                    }

                    //unfreeze object rotation
                    grabbedObject.gameObject.GetComponent<Rigidbody2D>().freezeRotation = false;

                }
                //otherwise update the visuals of the grabbed object
                else
                {
                    s.UpdateVisual();
                }
            }

            //Charge if on ground, not grabbing, not charging clap, pressing E
            if (isOnGround && !isAttachedGrab && Input.GetKey(KeyCode.E))
            {
                if (!isChargingReveal)
                {
                    //play particles
                    revealParticles.Play();

                    //ANIMATION: UNCOMMENT FOR CHARGE
                    spriteAnim.SetBool("isCharging", true);
                }
                //we are charging
                isChargingReveal = true;
                //increment timer only if we are under max
                if (revealTimer >= maxReveal)
                {
                    revealTimer = maxReveal;
                    //stop particles
                    revealParticles.Stop();
                }
                else
                {
                    revealTimer += Time.deltaTime;
                }
            }
            //otherwise if we have charged a sound and released E, do it.
            else if (isChargingReveal && !Input.GetKey(KeyCode.E))
            {

                //ANIMATION: UNCOMMENT FOR CHARGE
                spriteAnim.SetBool("isCharging", false);

                //stop particles
                revealParticles.Stop();

                //no longer charging
                isChargingReveal = false;
                
                //discard small sounds
                if (revealTimer >= minReveal)
                { 
                    float revealDist = 0.6f; //how far away from the player
                    //calculate sound location
                    Vector2 revealPos = new Vector2(this.transform.position.x + (flipX ? -1f : 1f) * revealDist, this.transform.position.y);

                    //clap with the timer charge * vol multiplier
                    PlaySound(revealTimer * revealVolMult, revealPos, revealClip);
                }
                //reset timer
                revealTimer = 0f;
            }
        }
        //fade out character alpha to animate door transition
        else
        {
            //if an object is visible, decrease it's alpha until it reaches 0
            if (sprite.color.a > 0)
            {
                float newA = sprite.color.a - (1.5f * Time.deltaTime);
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newA);
            }
        }
    }
}

