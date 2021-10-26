using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakFloorDetectorBehavior : MonoBehaviour
{
    //reference to break floor script
    BreakFloorBehavior s;

    // Start is called before the first frame update
    void Start()
    {
        //update refs
        s = transform.parent.GetComponent<BreakFloorBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        //if object is a physics object or player, break the floor
        string t = coll.gameObject.tag;
        if ((t == "Player" || t == "Phys") && !s.GetBroken())
        {
            s.SetBroken(true);
            s.Break();
        }
    }
}
