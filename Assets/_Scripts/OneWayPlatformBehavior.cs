using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformBehavior : MonoBehaviour
{
    //vars
    EdgeCollider2D box = null;

    // Start is called before the first frame update
    void Start()
    {
        box = this.GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraBehavior s = Camera.main.GetComponent<CameraBehavior>();
        box.enabled = (s.player.position.y-0.9 >= this.transform.position.y);
    }
}
