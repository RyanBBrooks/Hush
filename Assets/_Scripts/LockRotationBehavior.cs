using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotationBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //lock rotation of a physics object that enters this.
    private void OnTriggerEnter2D(Collider2D col)
    {
        //if the object is tagged as physics
        GameObject o = col.gameObject;
        if(o.tag == "Phys")
        {
            //lock its rotation
            Rigidbody2D rb = o.GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }
    }
}
