using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyScript : MonoBehaviour
{

    public float horizVelocity = 10F;
    public float vertVelocity = 10F;
    
    Rigidbody2D rb;
   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    

    void Update()
    {
  
       

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(x * horizVelocity, y * vertVelocity);

  

    }
}

