using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyScript : MonoBehaviour
{

    public LayerMask ground;
    public float speed = 1F;
    public float smoothing = 20f;
    public float jumpForce = 5f;
    bool onGround = false;
    bool jumping = false;
    private Vector3 vel = Vector3.zero;
    Rigidbody2D body;
    SpriteRenderer sprite;
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
        }
    }
    private void OnCollisionStay2D(Collision2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            SoundPhysicsObject s = o.gameObject.GetComponent<SoundPhysicsObject>();
            s.UpdateVisual();
        }
    }

    void Update()
    {
        //Stop Jumping
        if (onGround && jumping)
        {
            jumping = false;
        }

        //Horizontal Movement
        float x = Input.GetAxis("Horizontal");
        //Prevent Undermoving
        if (Mathf.Abs(x) < 0.2)
        {
            x = 0;
        }
        //Update Velocity
        Vector3 target = new Vector2(x * speed, body.velocity.y);
        body.velocity = Vector3.SmoothDamp(body.velocity, target, ref vel, smoothing);
        //flip sprite
        anim.SetFloat("Speed", Mathf.Abs(x));

        if (x < 0 && body.velocity.x < 0)
        {
            sprite.flipX = true;
        }
        if (x > 0 && body.velocity.x > 0)
        {
            sprite.flipX = false;
            

        }

        //Jumping
        if (
            (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) &&
            onGround)
        {
            onGround = false;
            jumping = true;
            body.AddForce(new Vector2(0f, jumpForce));
        }
        //Holding space increases float
        else if (
            !(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0)) &&
            !onGround &&
            jumping)
        {
            body.AddForce(new Vector2(0f, -1f));
        }
        body.AddForce(new Vector2(0f, -1f));

        //Debug.Log(body.velocity.x + "   " + body.velocity.y + "   " + onGround + "  " + jumping);
    }
}

