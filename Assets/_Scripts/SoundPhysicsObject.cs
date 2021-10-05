using UnityEngine;

public class SoundPhysicsObject : MonoBehaviour
{
    //visual layer
    public GameObject sprite;
    GameObject box;
    SpriteRenderer sprite_r;
    bool visible = false;
    public float stasis_a = 0.8f;
    public float stasisDecayRate = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        box = this.gameObject;
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

    public void BeginStasisAnim()
    {
        visible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlaySound(0.3f, collision.contacts[0].point);
    }

    public void PlaySound(float vol, Vector2 pos /**AudioClip sound**/)
    {
        //Play sound here
        SoundBehavior_Cam s = Camera.main.GetComponent<SoundBehavior_Cam>();
        s.SpawnSoundCircle(pos, vol);
    }
}
