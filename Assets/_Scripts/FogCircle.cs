using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogCircle : MonoBehaviour
{
    //delete time
    public float decayRate = 0.1f;
    public GameObject blue;
    public float volume = 0.5f;
    SpriteRenderer blueR;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.localScale = new Vector3(volume, volume, volume);
        blueR = blue.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (blueR.color.a <= 0)
        {
            Object.Destroy(this.gameObject);
        }
        blueR.color = new Color(0, 0, 1, blueR.color.a - (decayRate * Time.deltaTime));
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            FogPhysicsObject s = o.GetComponent<FogPhysicsObject>();
            s.UpdateVisual();
        }       
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            FogPhysicsObject s = o.GetComponent<FogPhysicsObject>();
            //s.PlayStationaryAnim();
        }
    }


}
