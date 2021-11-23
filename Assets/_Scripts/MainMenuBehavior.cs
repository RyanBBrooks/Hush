using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Ryan Brooks u1115093
public class MainMenuBehavior : MonoBehaviour
{
    Canvas canvas; //ui canvas
    public Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Restart()
    {
        PlayerPrefs.SetString("Progress", "Tutorial");
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Continue()
    {
        //save HighScore
        if (!PlayerPrefs.HasKey("Progress"))
        {
            PlayerPrefs.SetString("Progress", "Tutorial");
        }
        SceneManager.LoadScene(PlayerPrefs.GetString("Progress"), LoadSceneMode.Single);
    }
}

