using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    public float speed = 300f;
    public float waitTime = 1f;
    float waitTimer = 0;
    Vector3 targetPos = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        Camera cam = Camera.main;
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        //this.gameObject.transform.position;
        mousePos.z = 0;
        if (waitTimer <= 0)
        { 
            this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, mousePos, Time.deltaTime * speed);
        }
        else
        {
            waitTimer -= Time.deltaTime;
            this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, mousePos, Time.deltaTime * speed/100);
        }
        //update circle to mouse click !!!
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            waitTimer = waitTime;
            targetPos = mousePos;
            SoundBehavior_Cam s = cam.GetComponent<SoundBehavior_Cam>();
            s.SpawnSoundCircle(this.gameObject.transform.position, 0.6f);
        }
    }
}
