using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Ryan Brooks u1115093
public class MainMenuBehavior : MonoBehaviour
{
    Canvas canvas; //ui canvas
    public Button[] buttons; //ui buttons array

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //if we click the NEW GAME button
    public void Restart()
    {
        //reset progress and load first level
        PlayerPrefs.SetString("Progress", "Level1_Tutorial");
        SceneManager.LoadScene("Level1_Tutorial", LoadSceneMode.Single);
    }

    //if we want to exit the game
    public void Exit()
    {
        Application.Quit();
    }

    //if we hit CONTINUE
    public void Continue()
    {
        //load progress if progress exists, otherwise load level1
        if (!PlayerPrefs.HasKey("Progress"))
        {
            PlayerPrefs.SetString("Progress", "Level1_Tutorial");
        }
        SceneManager.LoadScene(PlayerPrefs.GetString("Progress"), LoadSceneMode.Single);
    }
}

