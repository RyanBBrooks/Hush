using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsBehavior : MonoBehaviour
{
    //translation speed
    public float speed = 0.5f;
    public float stoppingPoint = 2300f;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(this.transform.position.y + speed > stoppingPoint)
        {
            if (timer > 4000)
            {
                SceneManager.LoadScene("Level0_TitleScene", LoadSceneMode.Single);
            }
            else
            {
                timer++;              
            }

            if (timer > 3000)
            {
                this.transform.GetChild(0).gameObject.GetComponent<Text>().color -= new Color(0, 0, 0, 0.001f);
            }
        }
        else
        {
            this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y + speed);
        }
    }
}
