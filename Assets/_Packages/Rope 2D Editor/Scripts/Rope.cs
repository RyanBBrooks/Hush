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
    public bool hasVisualLayer = false;
    GameObject visualRope;
    // Use this for initialization
    void Start()
    {
        //set up visual layer
        if (hasVisualLayer)
        {
            visualRope = GameObject.Instantiate(this.gameObject);
            visualRope.GetComponent<Rope>().hasVisualLayer = false;
            visualRope.GetComponent<Rope>().HangFirstSegment = visualRope.GetComponent<Rope>().HangLastSegment = false;
            for (int i = 0; i < visualRope.transform.childCount; i++)
            {
                Transform link = visualRope.transform.GetChild(i);
                link.GetComponent<Rigidbody2D>().simulated = false;
                link.GetComponent<SpriteRenderer>().forceRenderingOff = true;
            }
        }

        //make all ropes black
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
        }

        //test function
        if (hasVisualLayer)
        {
            ChangeVisiblity(false);
        }



    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeVisiblity(bool visible)
    {

        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().forceRenderingOff = !visible;
            Transform link = visualRope.transform.GetChild(i);
            link.GetComponent<SpriteRenderer>().forceRenderingOff = visible;
            link.transform.SetPositionAndRotation(this.gameObject.transform.GetChild(i).position, this.gameObject.transform.GetChild(i).rotation);
        }
    }

    public void ChangeVisualColor(Color c)
    {
        for (int i = 0; i < visualRope.transform.childCount; i++)
        {
            visualRope.transform.GetChild(i).GetComponent<SpriteRenderer>().color = c;
        }
    }
}

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        