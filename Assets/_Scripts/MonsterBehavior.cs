using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    public float smoothing = 0.01f; //movement smoothing

    SpriteRenderer sprite;
    
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
        float step = speed * Time.deltaTime; // calculate distance to move
        float y = -1.644f;
        Vector3 newPos = new Vector3(transform.position.x - speed/100, transform.position.y, transform.position.z);

        if ("Level1_Tutorial" == SceneManager.GetActiveScene().name)
        {
            Debug.Log("yolo");
            if (transform.position.x > 26.5)
            {
                y = -3.8f;
            }
            else
            {
                newPos = Vector3.MoveTowards(transform.position, lily.gameObject.transform.position, step);
            }
            
            if (transform.position.x < 26.5 && transform.position.x > 0)
            {
                this.gameObject.SetActive(false);
                transform.position = new Vector3(-7, -1.644f, 0);
                speed = 2;
            }
        }
        else if("Level5_Final_Level" == SceneManager.GetActiveScene().name)
        {
            y = 40;
            newPos = Vector3.MoveTowards(transform.position, lily.gameObject.transform.position, step);
            
        }

        float velocity = transform.position.x - newPos.x;
        transform.position = new Vector3(newPos.x, y, newPos.z);
        if (transform.position.x < 26.5 && transform.position.x > 0 && "Level1_Tutorial" == SceneManager.GetActiveScene().name)
        {
            this.gameObject.SetActive(false);
            transform.position = new Vector3(-7, -1.644f, 0);
            speed = 2;
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

    private void OnCollisionEnter(Collision collision)
    {
        GameObject o = collision.gameObject;
        if (o.tag == "Player")
        {
            body.velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject o = collision.gameObject;
        if(o.tag == "Player")
        {
            body.velocity = Vector3.zero;
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
