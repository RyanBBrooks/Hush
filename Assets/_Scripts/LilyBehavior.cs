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

    //sound vars
    public float stepVol = 0.25f; // the volume of a step

    // Start is called before the first frame update
    void Start()
    {
        //get components to initialize them
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255,255,255);
        spriteAnim = GetComponent<Animator>();
    }

    //Determine if lily is on the ground by checking the collision box
    private void OnTriggerExit2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.layer == groundLayer || o.layer == physLayer)
        {
            isOnGround = false;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.layer == groundLayer || o.layer == physLayer)
        {
            isOnGround = true;
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
            spriteAnim.SetBool("isPushing", false);
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
                    spriteAnim.SetBool("isPushing", true);
                    isWalkPushing = true;
                }
                else
                {
                    //stop any possible pushing animations, we have broke horizontal, grounded contact
                    isWalkPushing = false;
                    spriteAnim.SetBool("isPushing", false);
                }
            }
        }
    }

    void Update()
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

        //Update Velocity
        Vector3 target = new Vector2(x * m_speed, body.velocity.y);
        body.velocity = Vector3.SmoothDamp(body.velocity, target, ref vel, smoothing);

        //update speed to induce movement animation
        spriteAnim.SetFloat("Speed", Mathf.Abs(x));

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

        //Footstep EchoCircles & sounds
        if (isOnGround && isStepping && !hasStepped)
        {
            //Spawn an echo circle
            CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
            //set circle position at foot and slightly in front of character to account for movement speeed
            Vector2 stepLocation = new Vector2(this.gameObject.transform.position.x + body.velocity.x/20,
                                                this.gameObject.transform.position.y - this.gameObject.transform.localScale.y); ;
            s.SpawnEchoCircle(stepLocation, stepVol);

            //TODO : Play a footstep sound here!

            //reset vars
            isStepping = false; // in case this is the last tick of the frame
            hasStepped = true;
        }
        // a check since the step is updated every tick while animating
        else if (!isStepping)
        {
            hasStepped = false;
        }


        //prevent jumping if we are grabbing, if we press the jump button down, and are on ground
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) 
            && !isAttachedGrab && isOnGround)
        {
            isOnGround = false; //gaurentee we are off the ground once we start jumping
            body.AddForce(new Vector2(0f, jumpForce)); // jump
        }
        //Holding space increases float (we do this by adding a negative force while jumping and not holding space)
        else if (!(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0)) &&
            !isOnGround)
        {
            body.AddForce(new Vector2(0f, antiFloatForce)); //downward force applied
        }

        //Begin Grabbing Code
        //if we are holding the grab key, and we are not currently grabbing, and we are on the ground...
        if (Input.GetKey(KeyCode.LeftShift) && !isAttachedGrab && isOnGround)
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

                    //if object's rotation should be locked, do it
                    grabbedObject.gameObject.GetComponent<Rigidbody2D>().freezeRotation = s.GetGrabLocksRotation();

                    //set the anchor to the position where our ray hit the object relative to the origin of that object
                    j.anchor = Quaternion.Inverse(grabbedObject.transform.rotation) * ((Vector3)hit.point -  grabbedObject.transform.position);

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
    }
}

