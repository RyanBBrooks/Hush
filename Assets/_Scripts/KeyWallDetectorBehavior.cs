using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyWallDetectorBehavior : MonoBehaviour
{
    GameObject wall = null;

    // Start is called before the first frame update
    void Start()
    {
        //update refs
        wall = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //returns the wall 
    public GameObject getWall()
    {
        return wall;

    }
}
