using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    bool isJumping = false; //whether lily is jumping
    PolygonCollider2D hitBox; //lily's hitbox
    BoxCollider2D feetBox; //box for detecting the floor

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
    bool hasStepped = false; // whether lily has just stepped on previous frame

    //door vars
    bool isTransitioning = false; //are we doing a scene transition
    List<GameObject> keys = null; //current number of keys held by lily

    //Clap Reveal vars
    public float maxClapTimer = 3f; //how long to reset clap
    float clapTimer = 0f; //current value of timer
    Slider clapBarSlider = null; //slider bar
    Image clapBarImg = null; //image for slider bar
    Canvas clapBarCanvas = null; //canvas for holding bar
    bool isClapping = false; //whether lily is clapping

    //death vars
    bool dead = false;


    //sound vars
    public float stepVol = 0.25f; // the volume of a step
    public float clapVol = 2.6f; // the volume of a step
    AudioSource src;
    public List<AudioClip> footstepList; //list of footstep sounds
    public AudioClip clapClip; //clap
    public AudioClip fallClip; //hitting the ground
    public AudioClip deathClip; //dying


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
        clapBarCanvas = this.transform.GetChild(0).gameObject.GetComponent<Canvas>();
        clapBarSlider = clapBarCanvas.transform.GetChild(0).gameObject.GetComponent<Slider>();
        clapBarImg = clapBarSlider.transform.GetChild(0).gameObject.GetComponent<Image>();
        src = GetComponent<AudioSource>();
    }

    //return whether lily is clapping
    public void SetIsClapping(bool b)
    {
        isClapping = b;
    }

    //Determine if lily is on the ground by checking the collision box: FLOOR
    private void OnTriggerExit2D(Collider2D col)
    {
        //if the player is leaving a physics object or floor 
        GameObject o = col.gameObject;
        if (o.layer == groundLayer || o.layer == physLayer)
        {
            isOnGround = false; //lily is no longer on the ground
            spriteAnim.SetBool("isFalling", true); //update animation
        }

        
    }
    //check collisions for entering triggers: FLOOR/DOOR/LOCK WALL/DEATH
    private void OnTriggerStay2D(Collider2D col)
    {
        //if we are dead do not check
        if (dead) return;

        //lily is touching the ground
        GameObject o = col.gameObject;
        if (o.layer == groundLayer || o.layer == physLayer)
        {
            isOnGround = true; //lily is on the ground
            spriteAnim.SetBool("isFalling", false); //update animation

            //if lily was jumping, then update the value of jumping
            if (isJumping)
            {
                isJumping = body.velocity.y > 0.05;
            }
        }

        //if we are in front of a door trigger
        if (o.tag == "Door" && !isClapping)
        {
            //get the door script
            DoorBehavior s = o.GetComponent<DoorBehavior>();

            //if we press up and are on the ground
            if ((Input.GetKey(KeyCode.W) || Input.GetAxis("Vertical") > 0.5) && isOnGround)
            {
                //if the door is locked check if we have a key, use the key
                if (s.locked && keys.Count>0 && !s.getTargeted())
                {
                    KeyBehavior k = keys[keys.Count-1].GetComponent<KeyBehavior>();
                    s.setTargeted(true); //target the door
                    k.use(o); //use the key
                    keys.RemoveAt(keys.Count-1); //decrement key count           
                }

                //otherwise if the door is unlocked, go through the door
                else if (!s.locked)
                {
                    //set the player to not be able to move/interact
                    body.simulated = false;
                    hitBox.enabled = false;
                    feetBox.enabled = false;
                    isTransitioning = true;

                    //delete all keys - can't carry them between levels
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

        //if we are in front of a lock wall trigger
        if (o.tag == "KeyWall" && !isClapping)
        {
            //get the lock wall (automatically checks to see if it is broken)
            GameObject wall = o.GetComponent<LockWallDetectorBehavior>().getWall();
            LockWallBehavior s = wall.GetComponent<LockWallBehavior>();        
            
            //if we have a key, and the wall is not targeted by another key, use the key
            if (keys.Count > 0 && !s.getTargeted())
            {
                KeyBehavior k = keys[keys.Count - 1].GetComponent<KeyBehavior>();
                s.setTargeted(true); //target the wall
                k.use(wall); //use the key
                keys.RemoveAt(keys.Count - 1); //decrement key count
            }
        }

        //if we touch something that causes death -> then die
        if (o.tag == "Death")
        {
            Die();
        }

    }

    //detect entering a trigger: KEYS/MONSTER
    private void OnTriggerEnter2D(Collider2D col)
    {
        //if we are dead do not check
        if (dead) return;

        //if we come into contact with a key
        GameObject o = col.gameObject;       
        if (o.tag == "Key" && !keys.Contains(o))
        {
            KeyBehavior s = o.GetComponent<KeyBehavior>();

            //calculate new key follow target
            int n = keys.Count;
            GameObject t = this.gameObject;
            if (n > 0)
            {
                t = keys[n - 1];
            }
            s.collect(t); //collect the key
            keys.Add(o); //add the key to the list
        }

        //if we touch a monster trigger
        else if (o.tag == "MonsterTrigger")
        {
            //activate the monster trigger
            o.GetComponent<MonsterTriggerBehavior>().activate();
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
        //if it is a physics object
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            PhysicsObjectBehavior s = o.gameObject.GetComponent<PhysicsObjectBehavior>();

            //the object is no longer being updated
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
        //if it is a physics object
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            PhysicsObjectBehavior s = o.gameObject.GetComponent<PhysicsObjectBehavior>();

            //the object is being updated
            s.UpdateVisual();    

            //if we are not attached to an object, check if we are pushing it
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
        //do not kill the player if already dead
        if (dead) return;

        //set player to be dead
        dead = true;

        // play animation -(results in)-> load scene
        spriteAnim.SetBool("isDead", true);

        //delete all keys (just to be safe)
        foreach (GameObject key in keys)
        {
            KeyBehavior k = key.GetComponent<KeyBehavior>();
            k.delete();
        }
        keys.Clear();

        //play the death sound
        PlaySound(0.5f, transform.position, deathClip);
    }

    //plays a random sound from al list
    public void PlayRandomSound(float vol, Vector2 pos, List<AudioClip> clips)
    {
        int r = Random.Range(0, clips.Count); //pick a random index of the list
        PlaySound(vol, pos, clips[r]); //play a sound at index r
    }

    //plays a sound
    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //ignore broken clips
        if (!clip) return;

        //play sound
        src.PlayOneShot(clip, vol*6);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircleInExtents(pos, vol);
    }

    void Update()
    {
        //??

        //if (monsterAnimate.isActiveAndEnabled)
        //{
        //    Vector3 currPos = monsterAnimate.gameObject.transform.position;
        //    monsterAnimate.gameObject.transform.position = new Vector3(currPos.x - .1f, currPos.y, currPos.z); 
        //    if(currPos.x - .1 < 25)
        //    {
        //        monsterAnimate.gameObject.SetActive(false);
        //    }
        //}

        //if we are dead
        if (dead)
        {           
            //just slow down character motion
            Vector2 target = new Vector2(0, -5);
            body.velocity = Vector3.SmoothDamp(body.velocity, target, ref vel, smoothing);
            return;
        }

        //if we aren't transitioning levels
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
            if (isClapping)
            {
                m_speed = 0;
            }

            //Update Velocity
            float finalSpeed = x * m_speed;
            Vector2 target = new Vector2(finalSpeed, body.velocity.y);
            body.velocity = Vector3.SmoothDamp(body.velocity, target, ref vel, smoothing);

            //update speed to induce movement animation
            spriteAnim.SetFloat("Speed", Mathf.Abs(finalSpeed));

            //flip sprite according to movement direction if we aren't grabbing something
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

            //calculate pushing animation (direction) -> (push/pull)
            spriteAnim.SetFloat("PushDir", finalSpeed*(flipX? -1 : 1));

            //Footstep EchoCircles & sounds
            if (isOnGround && isStepping && !hasStepped)
            {
                //set circle position at foot and slightly in front of character to account for movement speeed
                Vector2 stepLocation = new Vector2(this.gameObject.transform.position.x + body.velocity.x / 20,
                                                    this.gameObject.transform.position.y - this.gameObject.transform.localScale.y); ;

                //TODO : Play a footstep sound here!
                PlayRandomSound(stepVol, stepLocation, footstepList);

                //reset vars
                isStepping = false; // in case this is the last tick of the frame
                hasStepped = true;
            }

            // a check since the step is updated every tick while animating
            else if (!isStepping)
            {
                hasStepped = false;
            }

            //prevent jumping if we are clapping
            //prevent jumping if we are grabbing, if we press the jump button down, and are on ground and are not jumping
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                && !isAttachedGrab && isOnGround && !isJumping && !isClapping)
            {
                isOnGround = false; //gaurentee we are off the ground once we start jumping.

                //isJumping = true; //leave this commented out

                spriteAnim.SetTrigger("jump");//update animation

                body.velocity = new Vector2(body.velocity.x, 0);//NEW set velocity to zero before jump in y dir to avoid high jumps

                body.AddForce(new Vector2(0f, jumpForce)); // jump
            }

            //Holding space increases float (we do this by adding a negative force while jumping and not holding space)
            else if (!(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0)) &&
                !isOnGround)
            {
                body.AddForce(new Vector2(0f, antiFloatForce)); //downward force applied if we arent holding space
            }

            //Begin Grabbing Code
            //if we are holding the grab key, and we are not currently grabbing, and we are on the ground, not clapping...
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Joystick1Button5)) && !isAttachedGrab && isOnGround && !isClapping)
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
                        if (s.GetGrabLocksRotation())
                        {
                            grabbedObject.gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
                        }

                        //set the anchor to the position where our ray hit the object relative to the origin of that object
                        j.anchor = Quaternion.Inverse(grabbedObject.transform.rotation) * ((Vector3)hit.point - grabbedObject.transform.position);

                        //set the distance to the difference between lily's position and the hit point + some modifier
                        j.distance = (hit.point - body.position).magnitude + jointDistMod;

                        //Debug.DrawLine(hit.point, body.position, Color.red, 2.5f, false); //<--- draws a line to test the drag distance leave commented out

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
                if (!isOnGround || (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Joystick1Button5)))
                {
                    //find the joint, disable it.
                    DistanceJoint2D j;
                    if ((j = grabbedObject.GetComponent<DistanceJoint2D>()) != null)
                    {
                        j.enabled = isAttachedGrab = false; //disable joint reset attachedgrab
                        s.SetVisible(false); //begin fade
                        spriteAnim.SetBool("isGrabbing", false); //update animation
                    }

                    //unfreeze object rotation
                    if (s.GetGrabLocksRotation())
                    {
                        grabbedObject.gameObject.GetComponent<Rigidbody2D>().freezeRotation = false;
                    }

                }
                //otherwise update the visuals of the grabbed object
                else
                {
                    s.UpdateVisual();
                }
            }

            //decrement clap timer over time counting down until it reaches 0
            if (clapTimer > 0)
            {
                clapTimer -= Time.deltaTime;
            }
            //hard cap lower bound to 0
            else
            {
                clapTimer = 0;
            }

            //update bar value based on clap timer
            clapBarSlider.value = clapTimer / maxClapTimer;

            //update color value until it reaches white, shake the bar if it is not white
            Color c = clapBarImg.color;
            if (c.g <= 1)
            {
                //update color value
                clapBarImg.color = new Color(1, c.g + Time.deltaTime, c.b + Time.deltaTime, 1);

                //shake bar -- intensity is relative to color
                clapBarCanvas.transform.localPosition = new Vector2(Mathf.Sin(Time.time * 5 * c.g/1) * 0.04f, 1.5f + Mathf.Sin(Time.time/1.3f * 5 * c.g/1) * 0.04f);
            }

            //clap if on ground, not grabbing, not charging clap, pressing E
            if (isOnGround && !isAttachedGrab && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button2)) && !isClapping)
            {
                //if timer is empty
                if (clapTimer <= 0) {
                    //set clapping vars
                    clapBarCanvas.transform.localPosition.Set(0, 1.5f, 0); //reset position of clap bar
                    clapBarImg.color = new Color(1, 1, 1, 1); //reset color of clap bar
                    isClapping = true;
                    spriteAnim.SetTrigger("clap"); //start animation
                    clapTimer = maxClapTimer; //reset timer
                    float clapDist = 0.6f; //how far away from the player is the clap (accounts for hand distance)

                    //calculate clap location
                    Vector2 clapPos = new Vector2(this.transform.position.x + (flipX ? -1f : 1f) * clapDist, this.transform.position.y);

                    //clap
                    PlaySound(clapVol, clapPos, clapClip);
                }
                //otherwise play animation to show you cant use it yet, automatically triggered by changing color
                else if (clapBarImg.color.g >= 1)
                {
                    //update bar color
                    clapBarImg.color = new Color(1, 0.45f, 0.45f, 1);                  
                }
            }
        }

        //Otherwise, we are transitioning levels. fade out character alpha to animate door transition
        else
        {
            //if visible, decrease lily's alpha until it reaches 0
            if (sprite.color.a > 0)
            {
                float newA = sprite.color.a - (1.5f * Time.deltaTime);
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newA);
            }
        }
    }
}

