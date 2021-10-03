using UnityEngine;

public class SoundPhysicsObject : MonoBehaviour
{
    //visual layer
    GameObject box; 
    GameObject sprite;
    SpriteRenderer sprite_r;
    bool visible = false;
    public float stasis_a = 0.8f;
    public float stasisDecayRate = 0.1f;
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
        if(!visible && sprite_r.color.a > stasis_a)
        {
            sprite_r.color = new Color(sprite_r.color.r, sprite_r.color.g, sprite_r.color.b, sprite_r.color.a - (stasisDecayRate * Time.deltaTime));
        }
    }

    public void UpdateVisual()
    {
            visible = true;
            sprite_r.color = new Color(sprite_r.color.r, sprite_r.color.g, sprite_r.color.b, 1);
            sprite.transform.position = box.transform.position;
    }

    public void PlayStasisAnim()
    {
        visible = false;
    }
}
