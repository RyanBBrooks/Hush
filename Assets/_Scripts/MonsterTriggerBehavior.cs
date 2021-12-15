using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTriggerBehavior : MonoBehaviour
{
    public GameObject monster;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate()
    {
        monster.SetActive(true);
        monster.GetComponent<MonsterBehavior>().activate();
        Destroy(this.gameObject);
    }
}
