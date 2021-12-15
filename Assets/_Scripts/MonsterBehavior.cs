using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MonsterBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public bool window;
    public GameObject lily;
    int groundLayer = 7; //layer mask referring to the ground layer (layer #7 should be)
    public AudioSource src;
    BoxCollider2D col;
    public float vol = 1;
    Rigidbody2D body;
    public float smoothing = 0.01f; //movement smoothing
    bool walking = false;
    float timer = 0;

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
        if (walking)
        {
            sprite.color = new Color(1, 1, 1, sprite.color.a + 0.02f);
            float step = speed * Time.deltaTime; // calculate distance to move
            Vector3 newPos = new Vector3(transform.position.x - speed / 100, transform.position.y, transform.position.z);

            float velocity = transform.position.x - newPos.x;
            transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
            Debug.Log(timer);
            timer += Time.deltaTime;
            if (window && timer>3)
            {
                Destroy(this.gameObject);
            }

            if (velocity < 0)
            {
                sprite.flipX = false;
            }
            else if (velocity > 0)
            {
                sprite.flipX = true;

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
        s.SpawnEchoCircleInExtents(pos, vol);
    }

    public void PlayRandomSound(float vol, Vector2 pos, List<AudioClip> clips)
    {
        int r = Random.Range(0, clips.Count); //calculate random list index r
        PlaySound(vol, pos, clips[r]); //play sound at r
    }
}
