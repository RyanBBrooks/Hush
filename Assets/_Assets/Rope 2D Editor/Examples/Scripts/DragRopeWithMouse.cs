using UnityEngine;
using System.Collections;

public class DragRopeWithMouse : MonoBehaviour {
    public Rope rope; 
	Transform firstSegment;
	// Use this for initialization
	void Start () {
		if (!rope) 
		{
			rope = GetComponent<Rope> ();
		}
        if (rope)
        {
            firstSegment = rope.transform.GetChild(0);
            firstSegment.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(rope)
	        {
	            Vector2 mousePoistion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	            //its better to make the first segment kinematic but it isn't required
	            firstSegment.GetComponent<Rigidbody2D>().MovePosition(mousePoistion);
	        }
		}
}
