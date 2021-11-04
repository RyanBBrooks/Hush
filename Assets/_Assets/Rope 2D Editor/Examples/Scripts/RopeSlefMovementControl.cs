using UnityEngine;
using System.Collections;

public class RopeSlefMovementControl : MonoBehaviour {
    public Rope rope;
	Transform firstSegment;
    // Use this for initialization
    void Start()
    {
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

    float elapsedTime = 0;
    // Update is called once per frame
    void Update () {
        elapsedTime += Time.deltaTime;
        if (rope)
        {
            //the first segment should be kinematic 
            firstSegment.GetComponent<Rigidbody2D>().velocity = new Vector2(2*Mathf.Sin(elapsedTime), 0);
        }
    }
}
