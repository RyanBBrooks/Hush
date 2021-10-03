using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCircle : MonoBehaviour
{
    //delete time
    public float decayRate = 0.1f;
    public float volume = 0.5f;
    public GameObject blue;  
    SpriteRenderer ring_r;
    SpriteRenderer blueR;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.localScale *= volume;
        ring_r = this.gameObject.GetComponent<SpriteRenderer>();
        this.gameObject.transform.localScale = new Vector3(volume, volume, volume);
        blueR = blue.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (blueR.color.a <= 0 || !ring_r.isVisible)
        {
            Object.Destroy(this.gameObject);
        }
        blueR.color = new Color(0, 0, 1, blueR.color.a - (decayRate * Time.deltaTime));
        ring_r.color = new Color(1, 1, 1, ring_r.color.a - (decayRate * Time.deltaTime));
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys") //TODO possibly implement second parameter that ensures the sound circle is visible on screem before updating position of objcts //this.gameObject.GetComponent<Renderer>().isVisible ?
        {
            SoundPhysicsObject s = o.transform.parent.gameObject.GetComponent<SoundPhysicsObject>();
            s.UpdateVisual();
        }       
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            SoundPhysicsObject s = o.transform.parent.gameObject.GetComponent<SoundPhysicsObject>();
            s.PlayStasisAnim();
        }
    }


}
