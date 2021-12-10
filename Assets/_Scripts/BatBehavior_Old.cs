using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ryan Brooks u1115093 _OUTDATED DO NOT USE!!!!!!!!
public class BatBehavior_Old : MonoBehaviour
{
    //movement vars
    public float speed = 5f;
    
    //screech vars
    public float screechDist = 0.5f; //minimum distance to screech from target
    public float screechVol = 0.65f;

    //targeting vars
    public int maxTargets = 6;
    public GameObject targetPrefab;
    public SpriteRenderer sprite;
    List<GameObject> targets = new List<GameObject>();

    //sound vars=    
    AudioSource src;//SOUND: Create one AudioSource variable for audio source
    public AudioClip screechClip;// screeching clip

    // Start is called before the first frame update
    void Start()
    {
        //get reference
        sprite = GetComponent<SpriteRenderer>();
        src = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //convert the mouse position to world space
        Camera cam = Camera.main;
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        //the bat shoud go to the mouse or the oldest target if one exists
        Vector2 newPos, currentPos = this.gameObject.transform.position;
        if (targets.Count==0)
        {
            newPos = Vector2.Lerp(currentPos, mousePos, Time.deltaTime * speed);
        }
        else
        {
            newPos = Vector2.Lerp(currentPos, targets[0].transform.position, Time.deltaTime * speed);
        }
        
        //flip sprite based on movement dir
        sprite.flipX = (currentPos - newPos).x > 0;

        //update position
        this.gameObject.transform.position = newPos;

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
        if (targets.Count>0 && (Vector3.Distance(targets[0].transform.position, this.gameObject.transform.position) <= screechDist && targets.Count > 0))
        {
            //TODO: play the actual bat audio file here
            PlaySound(screechVol, targets[0].transform.position, screechClip);

            //Delete the reached target
            Object.Destroy(targets[0]);
            targets.RemoveAt(0);
        }
    }
    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //UNCOMMENT ME ONCE CLIP EXISTS
        //src.PlayOneShot(clip, vol);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircleInExtents(pos, vol);
    }
}
