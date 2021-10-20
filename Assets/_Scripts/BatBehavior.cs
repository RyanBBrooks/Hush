using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ryan Brooks u1115093
public class BatBehavior : MonoBehaviour
{
    //movement vars
    public float speed = 5f;
    
    //screech vars
    public float screechDist = 0.5f; //minimum distance to screech from target
    public float screechVol = 0.65f;

    //targeting vars
    public int maxTargets = 6;
    public GameObject targetPrefab;
    List<GameObject> targets = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //convert the mouse position to world space
        Camera cam = Camera.main;
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        //the bat shoud go to the mouse or the oldest target if one exists
        if (targets.Count==0)
        { 
            this.gameObject.transform.position = Vector2.Lerp(this.gameObject.transform.position, mousePos, Time.deltaTime * speed);
        }
        else
        {
            this.gameObject.transform.position = Vector2.Lerp(this.gameObject.transform.position, targets[0].transform.position, Time.deltaTime * speed);
        }

        //If we click, set a target
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //destroy the oldest target if we are over the max
            if (targets.Count >= maxTargets)
            {
                Object.Destroy(targets[0]);
                targets.RemoveAt(0);
            }

            //create a target and add it to the list
            GameObject t = Instantiate(targetPrefab, mousePos, Quaternion.identity) as GameObject;
            targets.Add(t);          
        }

        //If we approximately reach the target, then deal with screeching
        if (Vector3.Distance(targets[0].transform.position, this.gameObject.transform.position) <= screechDist && targets.Count > 0)
        {
            //TODO: play the actual bat audio file here

            //Create a EchoCircle at the sound location
            CameraBehavior s = cam.GetComponent<CameraBehavior>();
            s.SpawnEchoCircle(targets[0].transform.position, screechVol);

            //Delete the reached target
            Object.Destroy(targets[0]);
            targets.RemoveAt(0);
        }
    }
}
