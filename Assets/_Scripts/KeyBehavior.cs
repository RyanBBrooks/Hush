using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteToParticlesAsset;

public class KeyBehavior : MonoBehaviour
{
    //animator
    SpriteRenderer rend = null;
    CircleCollider2D coll = null;
    ParticleSystem part = null;

    // Start is called before the first frame update
    void Start()
    {
        coll = this.GetComponent<CircleCollider2D>();
        rend = this.GetComponent<SpriteRenderer>();
        part = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Collect()
    {
        part.Play();
        coll.enabled = rend.enabled = false;
    }
}
