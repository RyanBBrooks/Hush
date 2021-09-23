using UnityEngine;

public class FogPhysicsObject : MonoBehaviour
{
    //visual layer
    public GameObject phys;
    public GameObject vis;
    SpriteRenderer visRend;
    // Start is called before the first frame update
    void Start()
    {
        visRend = vis.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVisual()
    {
            visRend.color = new Color(visRend.color.r, visRend.color.g, visRend.color.b, 255);
            vis.transform.position = phys.transform.position;
    }
}
