using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBehavior_Cam : MonoBehaviour
{
    public GameObject circle;
    public float osb = 1.5f; //outer screen buffer
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SpawnSoundCircle(Vector2 pos, float volume)
    {
        //find camera boundries
        Camera cam = Camera.main;
        Vector3 camPos = cam.transform.position;
        float camHhalf = Camera.main.orthographicSize;
        float camWhalf = camHhalf * Screen.width / Screen.height;
        float xMin = camPos.x - camWhalf;
        float xMax = camPos.x + camWhalf;
        float yMin = camPos.y - camHhalf;
        float yMax = camPos.y + camHhalf;

        //if within certain range of boundries, spawn anti-fog circle
        if (xMin - osb <= pos.x &&
            xMax + osb >= pos.x &&
            yMin - osb <= pos.y &&
            yMax + osb >= pos.y)
        {
            GameObject c = Instantiate(circle, pos, Quaternion.identity) as GameObject;
            c.GetComponent<SoundCircle>().volume = volume;
            //Debug.Log(c + " " + pos + " " + c.transform.position + " " + c.GetComponent<SoundCircle>().volume);
        }
    }
}
