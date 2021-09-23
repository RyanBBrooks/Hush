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
        //update circle to mouse click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            Instantiate(circle, pos, Quaternion.identity);
        }
        //update color over time
        else
        {
            //if (circle.color.r > 0.3)
           // {
            //    circle.color = new Color(circle.color.r - 0.001f, circle.color.g - 0.001f, circle.color.b - 0.001f, 1);
           // }
        }
        //print(circle.color);
    }
}
