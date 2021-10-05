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
    public float AirRes = 5;
    Vector3 velocity = new Vector3(0, 0, 0);
    float acc = 0;
    float dToGrnd = 0;
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

    bool IsOnGround()
    {
        Debug.DrawRay(transform.position, Vector2.down, Color.black);
        RaycastHit2D hit = 
            Physics2D.Raycast(transform.position, Vector2.down, dToGrnd + 0.01f, ground);
        return hit.collider != null;
       
    }
    

    void Update()
    {
        
        float x = Input.GetAxis("Horizontal");
        //if player not horizontally moving
        //horizontal movement code
        bool grounded = IsOnGround();
        Debug.Log(grounded + "  " + dToGrnd + "  " + x);

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
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            velocity.y = JumpImpulse + velocity.x;
        }
        else if (!grounded) 
        {
            velocity.y = rb.velocity.y - AirRes;
        }
        else
        {
            velocity.y = rb.velocity.y;
        }
        if (Input.GetKey("space") && !grounded)
        {
        }

        rb.velocity = velocity;
        
    }
}

