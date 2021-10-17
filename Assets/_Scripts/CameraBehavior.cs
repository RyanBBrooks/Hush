using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ryan Brooks u1115093
public class CameraBehavior : MonoBehaviour
{
    //vars
    public GameObject echoCirclePrefab; //instance of the echo circle
    public float outerScreenBuffer = 1.5f; //how far sounds "count" outside of the screen

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    //spawn an echo circle with a given volume and position
    public void SpawnEchoCircle(Vector2 pos, float volume)
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

        //if position within certain range of boundries, spawn EchoCircle
        if (xMin - outerScreenBuffer <= pos.x &&
            xMax + outerScreenBuffer >= pos.x &&
            yMin - outerScreenBuffer <= pos.y &&
            yMax + outerScreenBuffer >= pos.y)
        {
            GameObject c = Instantiate(echoCirclePrefab, pos, Quaternion.identity) as GameObject;
            c.GetComponent<EchoCircleBehavior>().volume = volume;
        }
    }
}
