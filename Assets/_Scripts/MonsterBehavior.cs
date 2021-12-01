using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public GameObject lily;
    int groundLayer = 7; //layer mask referring to the ground layer (layer #7 should be)
    public bool roar = false;
    public AudioSource audioSource;
    public float vol;
    public float height = 20;
    Rigidbody2D body;
    private Vector3 vel = Vector3.zero;
    public float smoothing = 0.01f; //movement smoothing

    SpriteRenderer sprite;
    bool flipX = false;
    Animator animate;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animate = GetComponent<Animator>();
        animate.SetBool("walk", true);
    }

    
    // Update is called once per frame
    void Update()
    {
        animate.SetFloat("Speed", speed);
        float step = speed * Time.deltaTime; // calculate distance to move
            Vector3 newPos = Vector3.MoveTowards(transform.position, lily.gameObject.transform.position, step);
        transform.position = new Vector3(newPos.x, -1.6442f, newPos.z);
            float x = this.transform.position.x;
            if (x < 0 && body.velocity.x < 0)
            {
                sprite.flipX = flipX = true;
            }
            if (x > 0 && body.velocity.x > 0)
            {
                sprite.flipX = flipX = false;
            }

    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject o = collision.gameObject;
        if (o.tag == "Player")
        {
            speed = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        speed = 3;
    }

    //private void screech()
    //{
    //    Debug.Log("sceam");
    //    Spawn an echo circle
    //    CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
    //    set circle position at foot and slightly in front of character to account for movement speeed
    //    Vector2 location = new Vector2(this.gameObject.transform.position.x + height,
    //                                        this.gameObject.transform.position.y - this.gameObject.transform.localScale.y); ;
    //    s.SpawnEchoCircle(location, vol);
    //    audioSource.Play();

    //}


}
