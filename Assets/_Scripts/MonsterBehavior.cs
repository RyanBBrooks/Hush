using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MonsterBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public bool window;
    int groundLayer = 7; //layer mask referring to the ground layer (layer #7 should be)
    AudioSource src;
    BoxCollider2D col;
    public float screamVol = 0.5f;
    Rigidbody2D body;
    public float smoothing = 0.01f; //movement smoothing
    bool walking = false;
    float deleteTimer = 0;
    float currentSpeed = 5;
    float echoTimer = 0;
    Vector2 headPos = Vector2.zero;

    public List<AudioClip> screamClips;

    SpriteRenderer sprite;
    
    Animator animate;
    void Start()
    {
        col = GetComponent<BoxCollider2D>();      
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        src = GetComponent<AudioSource>();
        animate = GetComponent<Animator>();
        sprite.color = new Color(1, 1, 1, 0);
        body.simulated = false;
        if (col)
        {
            col.enabled = false;
        }
       
    }

    
    // Update is called once per frame
    void Update()
    {
        headPos = new Vector2(this.transform.position.x - 3, this.transform.position.y + 3);
        if (walking)
        {
            if (currentSpeed < speed)
            {
                currentSpeed += Time.deltaTime;
            }
            sprite.color = new Color(1, 1, 1, sprite.color.a + 0.02f);
            Vector3 newPos = new Vector3(transform.position.x - currentSpeed * Time.deltaTime, transform.position.y, transform.position.z);

            float velocity = transform.position.x - newPos.x;
            transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
            deleteTimer += Time.deltaTime;
            echoTimer += Time.deltaTime;
            //delete window after time elapse
            if (window && deleteTimer>3.2)
            {
                Destroy(this.gameObject);
            }
            //flip sprite
            if (velocity < 0)
            {
                sprite.flipX = false;
            }
            else if (velocity > 0)
            {
                sprite.flipX = true;

            }
            //add more circles
            if(echoTimer > 0.5)
            {
                CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
                s.SpawnEchoCircleInExtents(headPos, screamVol*4);
                echoTimer = 0;
            }
            //play another sound
            if (!src.isPlaying)
            {
                    PlayRandomSound(screamVol, headPos, screamClips);
            }


        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject o = collision.gameObject;
        if (o.tag == "Player" && walking)
        {
            body.velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject o = collision.gameObject;
        if(o.tag == "Player" && walking)
        {
            body.velocity = Vector3.zero;
        }
    }

    public void activate()
    {
        walking = true;
        body.simulated = true;
        if (col)
        {
            col.enabled = true;
        }
        animate.SetBool("walk", true);
      
    }

    public void PlaySound(float vol, Vector2 pos, AudioClip clip)
    {
        //UNCOMMENT ME ONCE CLIP EXISTS
        if (!src) return;
        src.PlayOneShot(clip, vol * 6);

        //spawn a "EchoCircle"
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        s.SpawnEchoCircleInExtents(pos, vol*4);
    }

    public void PlayRandomSound(float vol, Vector2 pos, List<AudioClip> clips)
    {
        int r = Random.Range(0, clips.Count); //calculate random list index r
        PlaySound(vol, pos, clips[r]); //play sound at rF
    }
}
