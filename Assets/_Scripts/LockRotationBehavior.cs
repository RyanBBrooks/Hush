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

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if(o.tag == "Phys")
        {
            Rigidbody2D rb = o.GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }
    }
}
