using UnityEngine;
using System.Collections;

public class CameraLookAt : MonoBehaviour
{
    public GameObject target;

    public float radius = 10;
    public float vel = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        float x = radius * Mathf.Cos(Time.time * vel);
        float y = radius * Mathf.Sin(Time.time * vel);
        transform.position = new Vector3(x, y, -10);

        if (transform && target!=null )
            transform.LookAt(target.transform);
    }
}
