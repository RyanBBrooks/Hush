using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabBehavior : MonoBehaviour
{
    BoxCollider2D box;
    LilyScript s;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("fyuasfkilcsadjklo;tnjkl;qwdaehjk; tydgfnzsjscdg,./hagevf");
        s = this.transform.parent.gameObject.GetComponent<LilyScript>();
        Debug.Log(s + "d");
        box = this.gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("fyuasfkilcsadjklo;tnjkl;qwdaehjk; tydgfnzsjscdg,./hagevf");
        GameObject o = col.gameObject;

        if (o.tag == "Phys")
        {
            
            s.SetTarget(null);
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        Debug.Log("fyuasfkilcsadjklo;tnjkl;qwdaehjk; tydgfnzsjscdg,./hagevf");
        if (o.tag == "Phys" )
        {
           
            s.SetTarget(o);
            
        }
    }

    public void flipX(bool b)
    {
        float bF = b ? 1 : 0;
        box.offset.Set(0.5f - bF, box.offset.y);
    }
}
