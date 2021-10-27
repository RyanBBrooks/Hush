using UnityEngine;
using System.Collections.Generic;
public enum SegmentSelectionMode
{
    RoundRobin,
    Random
}
public enum LineOverflowMode
{
    Round,
    Shrink,
    Extend
}
public class Rope : MonoBehaviour {
    public SpriteRenderer[] SegmentsPrefabs;
    public SegmentSelectionMode SegmentsMode;
    public LineOverflowMode OverflowMode; 
    [HideInInspector]
    public bool useBendLimit = true;
    [HideInInspector]
    public int bendLimit = 45;
    [HideInInspector]
    public bool HangFirstSegment = false;
    [HideInInspector]
    public Vector2 FirstSegmentConnectionAnchor;
    [HideInInspector]
    public Vector2 LastSegmentConnectionAnchor;
    [HideInInspector]
    public bool HangLastSegment = false;
#if UNITY_5_3_OR_NEWER
    [HideInInspector]
    public bool BreakableJoints=false;
    [HideInInspector]
    public float BreakForce = 100;
#endif
    [Range(-0.5f,0.5f)]
    public float overlapFactor;
    public List<Vector3> nodes = new List<Vector3>(new Vector3[] {new Vector3(-3,0,0),new Vector3(3,0,0) });
    [HideInInspector]
    public bool EnablePhysics=true;
    //toggle hush engine visualization
    public bool hasVisualLayer = false;
    // rope for hush engine visualization
    GameObject visualRope;
    // Use this for initialization
    void Start()
    {
        //set up visual layer
        if (hasVisualLayer)
        {
            //instantiate the visual rope
            visualRope = GameObject.Instantiate(this.gameObject);

            //update settings of visual rope
            visualRope.GetComponent<Rope>().hasVisualLayer = false;
            visualRope.GetComponent<Rope>().HangFirstSegment = visualRope.GetComponent<Rope>().HangLastSegment = false;

            //set original to have physics, visual to be an assortment of links
            for (int i = 0; i < visualRope.transform.childCount; i++)
            {
                Transform link = visualRope.transform.GetChild(i);
                link.GetComponent<Rigidbody2D>().simulated = false;
                this.transform.GetChild(i).GetComponent<SpriteRenderer>().forceRenderingOff = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //update location of visual rope to physical rope
    public void UpdateVisual()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            Transform link = visualRope.transform.GetChild(i);
            link.transform.SetPositionAndRotation(this.gameObject.transform.GetChild(i).position, this.gameObject.transform.GetChild(i).rotation);
        }
    }

    //update alpha value of visual rope
    public void ChangeVisualColorAlpha(float a)
    {
        for (int i = 0; i < visualRope.transform.childCount; i++)
        {
            SpriteRenderer rend = visualRope.transform.GetChild(i).GetComponent<SpriteRenderer>();
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, a);
        }
    }
}

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        