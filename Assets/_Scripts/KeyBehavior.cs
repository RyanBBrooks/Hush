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
    GameObject target = null;
    bool collected = false;
    DoorBehavior door = null;

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
        if (collected)
        {
            float speed = 5f;
            float dst = 1.5f;
            Vector2 keyPos = this.gameObject.transform.position;
            Vector2 targetPos = target.transform.position;
            //if collected follow lily
            if ((((Vector3.Distance(keyPos, targetPos)) >= dst) || door))
            {
                this.gameObject.transform.position = Vector2.Lerp(keyPos, targetPos, Time.deltaTime * speed);
            }

            //use key if near door
            if (door && (Vector3.Distance(keyPos, targetPos)) < 0.1)
            {
                delete();
                door.locked = false;
                door = null;
            }
        }
    }

    public void collect(GameObject t)
    {
        //TODO: play key sound (no visualization)

        rend.sortingLayerName = "Character";
        coll.enabled = false;
        target = t;
        collected = true;
        part.Play();

    }

    public void use(GameObject o)
    {
        //save door and target door
        if (door = o.GetComponent<DoorBehavior>())
        {
            target = o;
        }
    }

    public void delete()
    {
        part.Play();
        rend.enabled = false;
    }
}
