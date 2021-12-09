using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Ryan Brooks u1115093
public class CameraBehavior : MonoBehaviour
{
    //vars
    public GameObject echoCirclePrefab; //instance of the echo circle
    Canvas canvas; //ui canvas
    public float outerScreenBuffer = 1.5f; //how far sounds "count" outside of the screen
    public Button[] buttons;

    //movement vars
    public Transform player; // usually the player
    public float smoothing = 0.12f;

    public bool enableUI = true;
    public bool saveProgress = true;

    // Start is called before the first frame update
    void Start()
    {
        //disable all visuals
        canvas = transform.GetChild(0).gameObject.GetComponent<Canvas>();
        EnableUI(false);
        //set to default player position
        transform.position = player.transform.position;
        if (saveProgress) {
            PlayerPrefs.SetString("Progress", SceneManager.GetActiveScene().name);
        }

    }

    private void FixedUpdate()
    {
        //Do Camera Movement
        Vector2 targetPos = player.position;
        Vector2 newPos = Vector3.Lerp(transform.position, targetPos, smoothing);
        transform.position = new Vector3(newPos.x, newPos.y,-40);
    }

    // Update is called once per frame
    void Update()
    {

        //menu
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7)) && enableUI)
        {
            //make menu visible
            EnableUI(!canvas.enabled);
        }
    }

    public void EnableUI(bool e)
    {
        //show / hide UI
        canvas.enabled = e;

        //pause / unpause
        Time.timeScale = e ? 0 : 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void Exit()
    {
        SceneManager.LoadScene("Level0_TitleScene", LoadSceneMode.Single);
    }

    public void Continue()
    {
        EnableUI(false);
    }


    public Vector4 getExtents()
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
        return new Vector4(xMin, xMax, yMin, yMax);
    }

    //spawn an echo circle with a given volume and position
    public void SpawnEchoCircleInExtents(Vector2 pos, float volume)
    {
        Vector4 extents = getExtents();

        //if position within certain range of boundries, spawn EchoCircle
        if (extents.x - outerScreenBuffer <= pos.x &&
            extents.y + outerScreenBuffer >= pos.x &&
            extents.z - outerScreenBuffer <= pos.y &&
            extents.w + outerScreenBuffer >= pos.y)
        {
            SpawnEchoCircle(pos, volume);
        }
    }

    //spawn an echo circle with a given volume and position
    public void SpawnEchoCircle(Vector2 pos, float volume)
    {
        GameObject c = Instantiate(echoCirclePrefab, pos, Quaternion.identity) as GameObject;
        c.GetComponent<EchoCircleBehavior>().volume = volume;
    }
}
