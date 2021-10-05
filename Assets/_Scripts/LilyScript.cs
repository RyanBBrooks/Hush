using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyScript : MonoBehaviour
{

    public LayerMask ground;
    public float maxHVel = 1F;
    public float HAcc = 5f;
    public float HDecc = 40f;
    public float JumpImpulse = 100f;
    Vector3 velocity = new Vector3(0, 0, 0);
    float acc = 0;
    float dToGrnd = 0;
    bool onGround = false;
    Rigidbody2D rb;
    Collider2D coll;
   

    // Start is called before the first frame update
    void Start()
    {
        acc = HDecc;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        dToGrnd = coll.bounds.extents.y;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.layer == 7)
        {
            onGround = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.layer == 7)
        {
            onGround = true;
        }
    }


    void Update()
    {
        
        float x = Input.GetAxis("Horizontal");
        //if player not horizontally moving
        //horizontal movement code

        //Debug.Log(onGround + "  " + dToGrnd + "  " + x);

        if (Mathf.Abs(x) > 0.1) { 
            velocity.x = x * maxHVel;
            acc = HAcc;
        }
        else
        {
            velocity.x = 0;
            acc = HDecc;
        }
        //update x vel
        velocity.x = Mathf.Lerp(rb.velocity.x, velocity.x, Time.deltaTime * acc);

        //jump code
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            velocity.y = JumpImpulse + Mathf.Abs(velocity.x)/4;
        }
        else if (!onGround)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                velocity.y = rb.velocity.y;
            }
            else
            {
                velocity.y = rb.velocity.y;
            }
            
        }
        else
        {
            velocity.y = rb.velocity.y;
        }



        rb.velocity = velocity;

        Debug.Log(rb.velocity.x + "   " + rb.velocity.y);
    }
}

