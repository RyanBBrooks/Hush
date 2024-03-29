using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEyesBehavior : MonoBehaviour
{
    Animator anim = null;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //get a random r
        float r = Random.Range(0f, 1f);
        
        //if r small enough animate eyes
        if(r < 0.00051)
        {
            anim.SetTrigger("Blink");
        }
    }
}
