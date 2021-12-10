using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockWallDetectorBehavior : MonoBehaviour
{
    //this script serves to allow a second collision box (triger) for lock walls

    GameObject wall = null;

    // Start is called before the first frame update
    void Start()
    {
        //update reference to wall
        wall = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //returns the wall that is detected
    public GameObject getWall()
    {
        return wall;
    }
}
