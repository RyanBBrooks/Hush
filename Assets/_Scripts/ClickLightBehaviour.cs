using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickLightBehaviour : MonoBehaviour
{
    public GameObject circle;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //update circle to mouse click !!! eventually trigger this with sound at sound location
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            Instantiate(circle, pos, Quaternion.identity);
        }
    }
}
