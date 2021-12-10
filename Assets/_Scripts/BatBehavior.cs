using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ryan Brooks u1115093
public class BatBehavior : MonoBehaviour
{
    //movement vars
    public float speed = 2.5f; //bat movement speed
    public float waitTime = 2; //how long the bat waits at the targets
    float waitTimer = 0; //timer
    
    //screech vars
    public float screechDist = 0.5f; //minimum distance to screech from target
    public float defaultVolume = 0.65f; //screech volume if none is provided

    //targeting vars
    public SpriteRenderer sprite; //bat sprite
    List<Vector2> targets = new List<Vector2>(); //list of targets
    List<float> volumes = new List<float>(); //list of volumes

    //sound vars=   
    AudioSource src;
    public List<AudioClip> screechList;// screeching clip list

    // Start is called before the first frame update
    void Start()
    {
        //get variable refrences
        sprite = this.gameObject.GetComponent<SpriteRenderer>();
        src = GetComponent<AudioSource>();
    }

    //add target with a given volume
    public void addTarget(Vector2 t, float v)
    {
        //add t and v
        targets.Add(t);
        volumes.Add(v);

        //calculate the starting/ending position of the bat based on camera
        Vector2[] posns = calculateStartEnd();

        //set the beginning and end positions
        Vector2 startPos = posns[0];
        Vector2 endPos = posns[1];

        //if we are already close to the end position
        if (isWithinDist(endPos, 0.5f, this.transform.position))
        {
            //reset position to top left (start position)
            this.transform.position = startPos;
        }
    }

    //returns if s is Within dst of e
    bool isWithinDist(Vector2 s, float dst, Vector2 e)
    {
        return (Vector3.Distance(s, e)) <= dst;
    }

    //calcuate starting and ending positions
    Vector2[] calculateStartEnd()
    {
        //get extents of camera
        Camera cam = Camera.main;
        CameraBehavior s = cam.GetComponent<CameraBehavior>();
        Vector4 extents = s.getExtents();

        //calculate start and end pos
        Vector2 startPos = new Vector2(extents.x - 5, extents.w + 5);
        Vector2 endPos = new Vector2(extents.y + 5, extents.w + 5);

        //return them as an arr
        return new Vector2[] { startPos, endPos };
    }

    // Update is called once per frame
    void Update()
    {
        //count down timer if not at 0
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
        }

        //if we arent waiting for the timer
        else 
        {
            //begin position calculations - setup vars
            Vector2 newPos, currentPos = this.gameObject.transform.position;
            Vector2[] posns = calculateStartEnd();
            Vector2 endPos = posns[1];

            //if targets is empty and we have reached end position
            if (targets.Count <= 0 && isWithinDist(endPos, 0.5f, currentPos))
            {
                //reset position to the end
                newPos = endPos;
            }

            //if targets is empty or not near the end position : Do movement
            else
            {
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

            //flip sprite based on movement dir of the bat
            sprite.flipX = (currentPos - newPos).x > 0;

            //update position of the bat to the new position
            this.gameObject.transform.position = newPos;

            //If we approximately reach the target, then screech
            if (targets.Count > 0 && isWithinDist(targets[0], screechDist, this.gameObject.transform.position) && targets.Count > 0)
            {
                //determine volume
                float volume = defaultVolume;

                //if the volume is not negative, use the given volume. Otherwise use the default
                if (volumes[0] >= 0)
                {
                    volume = volumes[0];
                }

                //play the actual bat audio file
                PlayRandomSound(volume, targets[0], screechList);

                //start wait timer (pause bat at location)
                waitTimer = waitTime;

                //Delete the reached target and associated volume
                targets.RemoveAt(0);
                volumes.RemoveAt(0);
            }
        }
    }

    //plays a random sound from a list
    public void PlayRandomSound(float vol, Vector2 pos, List<AudioClip> clips)
    {
        int r = Random.Range(0, clips.Count); //calculate random list index r
        PlaySound(vol, pos, clips[r]); //play sound at r
    }

    //play a sound
    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //ignore invalid sounds
        if (!clip) return;

        //play sound
        src.PlayOneShot(clip, vol);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircle(pos, vol);
    }
}
