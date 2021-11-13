using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatTriggerBehavior : MonoBehaviour
{
    //vars
    public Vector2 targetPos = Vector2.zero;
    public float screechVol = 3;
    public bool useTriggerPosition = false; //use position of the object, overrides target pos
    public BatBehavior bat = null;


    // Start is called before the first frame update
    void Start()
    {
        if (useTriggerPosition)
        {
            targetPos = this.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject o = coll.gameObject;
        if(o.tag == "Player")
        {
            bat.addTarget(targetPos, screechVol);
            Destroy(this.gameObject);
        }
    }
}
