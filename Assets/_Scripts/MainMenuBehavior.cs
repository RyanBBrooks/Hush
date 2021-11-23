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
        SceneManager.LoadScene("TutorialLevel", LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Continue()
    {

    }
}

