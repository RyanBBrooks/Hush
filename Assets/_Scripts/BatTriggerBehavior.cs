using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ryan brooks
public class BatTriggerBehavior : MonoBehaviour
{
    //vars
    public Vector2 targetPos = Vector2.zero; //position of target
    public float screechVol = 3; //volume of associated screech
    public bool useTriggerPosition = false; //use position of the object, overrides target pos
    public BatBehavior bat = null; //bat to move


    // Start is called before the first frame update
    void Start()
    {
        //if we use the trigger position update the position of the target to be current location
        if (useTriggerPosition)
        {
            targetPos = this.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //if we touch the trigger
    private void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject o = coll.gameObject;
        if(o.tag == "Player")
        {
            bat.addTarget(targetPos, screechVol); //add the target to the controlled bat
            Destroy(this.gameObject); //destroy the trigger point
        }
    }
}
