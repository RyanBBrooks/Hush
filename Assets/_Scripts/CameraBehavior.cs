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
    public float outerScreenBuffer = 1.5f; //how far sounds "count" outside of the screen

    //movement vars
    public Transform player; // usually the player
    public float smoothing = 0.12f; //smoothing of the camera

    //ui vars
    Canvas canvas; //ui canvas
    public Button[] buttons; //buttons in the ui
    public bool enableUI = true; //is the ui enabled for this camera
    public bool saveProgress = true; //do we save progress at the start of this level

    //parallax vars
    public GameObject background; //background to be moved
    public float rate = 0.2f; //rate of parallax scroll

    // Start is called before the first frame update
    void Start()
    {
        //disable all visuals / UI
        canvas = transform.GetChild(0).gameObject.GetComponent<Canvas>();
        EnableUI(false);

        //set to default camera posn to  player position
        transform.position = player.transform.position;

        //if we save the game, save the game
        if (saveProgress) {
            PlayerPrefs.SetString("Progress", SceneManager.GetActiveScene().name);
        }

    }

    //camera movement
    private void FixedUpdate()
    {
        //Do Smooth Camera Movement
        Vector2 targetPos = player.position;
        Vector2 newPos = Vector3.Lerp(transform.position, targetPos, smoothing);
        transform.position = new Vector3(newPos.x, newPos.y,-40);
    }

    // Update is called once per frame
    void Update()
    {

        //enable the menu if we press the button and there is ui on this camera:
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7)) && enableUI)
        {
            //make UI menu visible/invisible based on current state
            EnableUI(!canvas.enabled);
        }

        //handle parallax movement
        background.transform.position = -rate * this.transform.position;
    }

    // enable/disable the ui
    public void EnableUI(bool e)
    {
        //show / hide UI
        canvas.enabled = e;

        //pause / unpause
        Time.timeScale = e ? 0 : 1;
    }

    //if we press restart reload this scene
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    //if we quit to menu load the menu scene
    public void Exit()
    {
        SceneManager.LoadScene("Level0_TitleScene", LoadSceneMode.Single);
    }

    //if we hit resume disable the ui
    public void Continue()
    {
        EnableUI(false);
    }

    //get the extents of the camera
    public Vector4 getExtents()
    {
        //find camera boundries
        Camera cam = Camera.main;
        Vector3 camPos = cam.transform.position;
        float camHhalf = Camera.main.orthographicSize; //half of the camera height
        float camWhalf = camHhalf * Screen.width / Screen.height; //half of the camera width

        //set max and min of camera bounds based on heights and widths
        float xMin = camPos.x - camWhalf;
        float xMax = camPos.x + camWhalf;
        float yMin = camPos.y - camHhalf;
        float yMax = camPos.y + camHhalf;

        //return positions as vector4
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
