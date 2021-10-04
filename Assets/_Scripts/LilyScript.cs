using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyScript : MonoBehaviour
{

    
    public float maxHVel = 1F;
    public float HAcc = 5f;
    public float HStopAcc = 20f;
    public float JumpImpulse = 10000f;
    public float AirRes = 5;
    Vector3 velocity = new Vector3(0, 0, 0);
    float acc = 0;
    float dToGrnd = 0;
    Rigidbody2D rb;
    Collider2D coll;
   

    // Start is called before the first frame update
    void Start()
    {
        acc = HStopAcc;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        dToGrnd = coll.bounds.extents.y;
    }

    bool IsOnGround()
    {
        return Physics.Raycast(transform.position, -Vector3.up, dToGrnd + 0.1f);
    }
    

    void Update()
    {
        
        float x = Input.GetAxis("Horizontal");
        //if player not horizontally moving
        //horizontal movement code
        bool grounded = IsOnGround();
        Debug.Log(grounded);
        if (Mathf.Abs(x) > 0.1) { 
            velocity.x = x * maxHVel;
            acc = HAcc;
        }
        else
        {
            velocity.x = 0;
            acc = HStopAcc;
        }
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            velocity.y = JumpImpulse;
        }
        if (Input.GetKey("space") && !grounded)
        {
        }
        

        rb.velocity = Vector3.Lerp(rb.velocity, velocity, Time.deltaTime * 10);
    }
}

