using UnityEngine;

public class FogPhysicsObject : MonoBehaviour
{
    //visual layer
    GameObject box; 
    GameObject sprite;
    SpriteRenderer sprite_r;
    // Start is called before the first frame update
    void Start()
    {
        box = this.transform.GetChild(0).gameObject;
        sprite = this.transform.GetChild(1).gameObject;
        sprite_r = sprite.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVisual()
    {
            sprite_r.color = new Color(sprite_r.color.r, sprite_r.color.g, sprite_r.color.b, 255);
            sprite.transform.position = box.transform.position;
    }
}
