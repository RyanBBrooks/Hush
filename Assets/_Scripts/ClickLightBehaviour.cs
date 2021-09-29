using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickLightBehaviour : MonoBehaviour
{
    public GameObject circle;
    public float osb = 1.5; //outer screen buffer
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
            //TODO only place a circle if this position is (close to being) on screen!
            Camera cam = Camera.main;
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            
            //find camera boundries
            Vector3 camPos = cam.transform.position;
            float camHhalf = Camera.main.orthographicSize;
            float camWhalf = camHhalf * Screen.width / Screen.height;          
            float xMin = camPos.x - camWhalf;
            float xMax = camPos.x + camWhalf;
            float yMin = camPos.y - camHhalf;
            float yMax = camPos.y + camHhalf;

            //if within certain range of boundries, spawn anti-fog circle
            if (xMin -osb <= pos.x &&
                xMax +osb >= pos.x &&
                yMin -osb <= pos.y &&
                yMax +osb >= pos.y)
            {

            Instantiate(circle, pos, Quaternion.identity);
            }
        }
    }
}
