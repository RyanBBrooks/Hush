using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyScript : MonoBehaviour
{

    public LayerMask ground;
    public float max_speed = 1F;
    public float smoothing = 20f;
    public float jumpForce = 5f;
    public float grabDist = 1f;
    public LayerMask pushMask;
    bool isGrabbing = false;
    bool flipx = false;
    bool onGround = false;
    bool isMoveInput = false; // relates to the pusing / pulling aniations, whether or not the player is trying to move
    bool isPushingNoGrab = false;
    private Vector3 vel = Vector3.zero;
    Rigidbody2D body;
    SpriteRenderer sprite;
    GameObject grabbed;
    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255,255,255);
        anim = GetComponent<Animator>();
    }

    //Determine if lily is on the ground
    private void OnTriggerExit2D(Collider2D col)
    {
        GameObject o = col.gameObject;

        if (o.layer == 7)
        {
            onGround = false;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.layer == 7)
        {
            onGround = true;
        }
    }

    //As long as lily is touching an object it will not fade, position will update
    private void OnCollisionExit2D(Collision2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            SoundPhysicsObject s = o.gameObject.GetComponent<SoundPhysicsObject>();
            s.BeginStasisAnim();

            //stop possible pushing animation
            anim.SetBool("isPushing", false);
            isPushingNoGrab = false;
        }
    }


    private void OnCollisionStay2D(Collision2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            SoundPhysicsObject s = o.gameObject.GetComponent<SoundPhysicsObject>();
            s.UpdateVisual();

            //if roughly horizontal to object, play pushing animation, otherwise stop the animation
            Vector2 avgNormal = Vector2.zero;
            foreach (ContactPoint2D contact in col.contacts)
            {
                avgNormal += contact.normal;
            }
            avgNormal /= col.contactCount; //normalize
            bool inHorizContact = Mathf.Abs(avgNormal.y) < 0.2;
            //Debug.Log(inHorizContact);
            if (!isGrabbing)
            {
                if (inHorizContact && onGround && isMoveInput)
                {
                    anim.SetBool("isPushing", true);
                    isPushingNoGrab = true;
                }
                else
                {
                    isPushingNoGrab = false;
                    anim.SetBool("isPushing", false);
                    // Debug.Log("STOP");
                }
            }
        }
    }

    void Update()
    {


        //Horizontal Movement
        float x = Input.GetAxis("Horizontal");
        //Prevent Undermoving
        if (Mathf.Abs(x) < 0.2)
        {
            x = 0;
            isMoveInput = false;
        }
        else
        {
            isMoveInput = true;
        }

        //cut speed in half if we are pulling something
        float m_speed = max_speed;
        if (isGrabbing || isPushingNoGrab)
        {
            //m_speed /= 2;
        }

        //Update Velocity
        Vector3 target = new Vector2(x * m_speed, body.velocity.y);
        body.velocity = Vector3.SmoothDamp(body.velocity, target, ref vel, smoothing);
        //flip sprite
        anim.SetFloat("Speed", Mathf.Abs(x));
        if (!isGrabbing)
        {
            if (x < 0 && body.velocity.x < 0)
            {
                sprite.flipX = flipx = true;
            }
            if (x > 0 && body.velocity.x > 0)
            {
                sprite.flipX = flipx = false;
            }

        }
        //Jumping
        if (!isGrabbing &&
            (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) &&
            onGround)
        {
            onGround = false;
            body.AddForce(new Vector2(0f, jumpForce));
        }
        //Holding space increases float
        else if (
            !(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0)) &&
            !onGround)
        {
            body.AddForce(new Vector2(0f, -3f));
        }
        body.AddForce(new Vector2(0f, -1f));

        //start Grabbing
        if (Input.GetKey(KeyCode.LeftShift) && !isGrabbing && onGround)
        {
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D hit = Physics2D.Raycast(
                                                this.transform.position, (flipx ? Vector2.left : Vector2.right) * transform.localScale.x,
                                                grabDist, pushMask);
            
            if(hit.collider != null)
            {
                
                grabbed = hit.collider.gameObject;
                DistanceJoint2D j;
                if ((j = grabbed.GetComponent<DistanceJoint2D>()) != null)
                {
                    j.enabled = true;
                    j.connectedBody = body;
                    Vector2 dist = j.anchor = Quaternion.Inverse(grabbed.transform.rotation) * ((Vector3)hit.point -  grabbed.transform.position);
                    j.distance = (hit.point - body.position).magnitude +0.1f;//(body.transform.position - grabbed.transform.position).magnitude;
                    //Debug.Log((Vector2)grabbed.transform.position - hit.point + "  " + hit.point + grabbed.transform.position);
                    Debug.DrawLine(hit.point, body.position, Color.red, 2.5f, false);
                    isGrabbing = true;
                }
            }
            Physics2D.queriesStartInColliders = true;
            isPushingNoGrab = false;
        }

        //update box visual if being pushed, stop grabbing if invialid
        if (isGrabbing)
        {
            SoundPhysicsObject s = grabbed.gameObject.GetComponent<SoundPhysicsObject>();
            if (!onGround || !Input.GetKey(KeyCode.LeftShift)){
                
                DistanceJoint2D j;
                if ((j = grabbed.GetComponent<DistanceJoint2D>()) != null)
                {
                    j.enabled = false;
                    isGrabbing = false;
                }

            }

            s.UpdateVisual();
            s.BeginStasisAnim();
        }

        //Debug.Log(isGrabbing + "    " + onGround + "   " );

    }
}

