using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ryan Brooks u1115093
public class BatBehavior : MonoBehaviour
{
    //movement vars
    public float speed = 5f;
    
    //screech vars
    public float screechDist = 0.3f;
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

        Camera cam = Camera.main;
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        //this.gameObject.transform.position;
        if (targets.Count==0)
        { 
            this.gameObject.transform.position = Vector2.Lerp(this.gameObject.transform.position, mousePos, Time.deltaTime * speed);
        }
        else
        {
            this.gameObject.transform.position = Vector2.Lerp(this.gameObject.transform.position, targets[0].transform.position, Time.deltaTime * speed);
        }
        //update circle to mouse click !!!
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (targets.Count >= maxTargets)
            {
                Object.Destroy(targets[0]);
                targets.RemoveAt(0);
            }
            GameObject t = Instantiate(targetPrefab, mousePos, Quaternion.identity) as GameObject;
            targets.Add(t);          
        }

        if (Vector3.Distance(targets[0].transform.position, this.gameObject.transform.position) <= screechDist && targets.Count > 0)
        {
            //play the sound here!!!
            CameraBehavior s = cam.GetComponent<CameraBehavior>();
            s.SpawnEchoCircle(targets[0].transform.position, screechVol);
            Object.Destroy(targets[0]);
            targets.RemoveAt(0);
        }



    }
}
