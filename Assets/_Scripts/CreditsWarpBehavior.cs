using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsWarpBehavior : MonoBehaviour
{
    CircleCollider2D over;
    // Start is called before the first frame update
    void Start()
    {
        over = this.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if we are touching the player
        GameObject o = collision.gameObject;
        if(o.tag == "Player")
        {
            SceneManager.LoadScene("Level6_Credits", LoadSceneMode.Single);
        }
    }
}
