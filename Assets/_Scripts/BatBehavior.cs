using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ryan Brooks u1115093
public class BatBehavior : MonoBehaviour
{
    //movement vars
    public float speed = 2.5f;
    public float waitTime = 2;
    float waitTimer = 0;
    
    //screech vars
    public float screechDist = 0.5f; //minimum distance to screech from target
    public float defaultVolume = 0.65f;

    //targeting vars
    public int maxTargets = 6;
    public GameObject targetPrefab;
    public SpriteRenderer sprite;
    List<Vector2> targets = new List<Vector2>();
    List<float> volumes = new List<float>();

    //sound vars=    
    AudioSource src;//SOUND: Create one AudioSource variable for audio source
    public AudioClip screechClip;// screeching clip

    // Start is called before the first frame update
    void Start()
    {
        //get variable refrences
        sprite = this.gameObject.GetComponent<SpriteRenderer>();
        src = GetComponent<AudioSource>(); // for sound
    }

    //add target with a given volume
    public void addTarget(Vector2 t, float v)
    {
        targets.Add(t);
        volumes.Add(v);
        Vector2[] posns = calculateStartEnd();
        Vector2 startPos = posns[0];
        Vector2 endPos = posns[1];
        if (isWithinDist(endPos, 0.5f, this.transform.position))
        {
            //reset position to top left
            this.transform.position = startPos;
        }
    }

    //returns if s is Within dst of e
    bool isWithinDist(Vector2 s, float dst, Vector2 e)
    {
        return (Vector3.Distance(s, e)) <= dst;
    }

    Vector2[] calculateStartEnd()
    {
        //get extents
        Camera cam = Camera.main;
        CameraBehavior s = cam.GetComponent<CameraBehavior>();



        Vector4 extents = s.getExtents();
        //calculate start and end pos
        Vector2 startPos = new Vector2(extents.x - 5, extents.w + 5);
        Vector2 endPos = new Vector2(extents.y + 5, extents.w + 5);
        return new Vector2[] { startPos, endPos };
    }

    // Update is called once per frame
    void Update()
    {
        //count down timer
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
        }
        //if we arent waiting 
        else 
        {
            Vector2 newPos, currentPos = this.gameObject.transform.position;
            Vector2[] posns = calculateStartEnd();
            Vector2 startPos = posns[0];
            Vector2 endPos = posns[1];

            //if targets is empty and we have reached end position
            if (targets.Count <= 0 && isWithinDist(endPos, 0.5f, currentPos))
            {
                //reset position to the end
                newPos = endPos;
            }
            //if targets is empty or not near the end position 
            else
            {
                //Do Movement

                //if we have targets go to those
                if (targets.Count > 0)
                {
                    newPos = Vector2.Lerp(currentPos, targets[0], Time.deltaTime * speed);
                }
                //otherwise go to end position
                else
                {
                    newPos = Vector2.Lerp(currentPos, endPos, Time.deltaTime * speed);
                }
            }

            //flip sprite based on movement dir
            sprite.flipX = (currentPos - newPos).x > 0;

            //update position
            this.gameObject.transform.position = newPos;
            //Debug.Log(newPos + " " + startPos + " " + endPos);

            //If we approximately reach the target, then deal with screeching
            if (targets.Count > 0 && isWithinDist(targets[0], screechDist, this.gameObject.transform.position) && targets.Count > 0)
            {
                float volume = defaultVolume;
                //determine volume
                if (volumes[0] >= 0)
                {
                    volume = volumes[0];
                }
                //TODO: play the actual bat audio file here
                PlaySound(volume, targets[0], screechClip);

                //start wait timer
                waitTimer = waitTime;

                //Delete the reached target
                targets.RemoveAt(0);
                volumes.RemoveAt(0);
            }
        }
    }
    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //UNCOMMENT ME ONCE CLIP EXISTS
        //src.PlayOneShot(clip, vol);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircle(pos, vol);
    }
}
