using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ryan Brooks u1115093
public class EchoCircleBehavior : MonoBehaviour
{
    //vars
    public float decayRate = 0.1f; //how quickly the circle decays
    public float volume = 0.5f; // the volume of the circle
    public GameObject blueCircle;  //reference to the blue circle 

    SpriteRenderer ringRenderer; // sprite rendere for the rings
    SpriteRenderer blueCircleRenderer; //renderer for the blue circle

    // Start is called before the first frame update
    void Start()
    {
        //set the size equal to the volume
        this.gameObject.transform.localScale = new Vector3(volume, volume, volume);

        //get references
        ringRenderer = this.gameObject.GetComponent<SpriteRenderer>();       
        blueCircleRenderer = blueCircle.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //fade out the alpha of the blue circle until it has an alpha of 0
        blueCircleRenderer.color = new Color(0, 0, 1, blueCircleRenderer.color.a - (decayRate * Time.deltaTime));
        
        //then delete it
        if (blueCircleRenderer.color.a <= 0)
        {
            Object.Destroy(this.gameObject);
        }

        //also fade the alpha of the rings
        ringRenderer.color = new Color(1, 1, 1, ringRenderer.color.a - (decayRate * Time.deltaTime));
    }

    //if it encounters a physics object, it updates the visual
    private void OnTriggerStay2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            PhysicsObjectBehavior s = o.gameObject.GetComponent<PhysicsObjectBehavior>();
            s.UpdateVisual();
        }
    }

    //if it fades away or the physics object leaves the range, we set it's visiblity to false and begin the fade anim
    private void OnTriggerExit2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.tag == "Phys")
        {
            PhysicsObjectBehavior s = o.gameObject.GetComponent<PhysicsObjectBehavior>();
            s.SetVisible(false);
        }
    }


}
