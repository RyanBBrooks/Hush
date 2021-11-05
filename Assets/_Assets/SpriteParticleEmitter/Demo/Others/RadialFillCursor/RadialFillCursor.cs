using UnityEngine;

public class RadialFillCursor : MonoBehaviour
{
    [Header("Radial Data")]
    public float radius = 8;
    public float strenght = 1;
    public float strenghtMax = 10;
    public float angle = 90;
    public float rotationAngle = -45;

    [Header("Input Speed")]
    public float radiusSpeed = 8;
    public float strenghtSpeed = 8;
    public float rotationSpeed = 80;
    public float angleSpeed = 80;

    [Header("Mesh Data")]
    public int meshAngleSeparation = 50;

    public MeshFilter meshFilter;
    public Color centerColor;
    public Color externalColor;
    
    private Vector3[] vertices;
    private Color[] colors;
    private int[] triangles;
    //private Vector2[] uvs;

    Vector3 tempV3 = Vector3.right;
    
	void Update () 
    {
	    Vector3 screenPoint = Input.mousePosition;
	    screenPoint.z = 10.0f; //distance of the plane from the camera
	    transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

	    if (Input.GetKey(KeyCode.W))
	        radius += Time.deltaTime * radiusSpeed;
	    else if (Input.GetKey(KeyCode.S))
	        radius -= Time.deltaTime * radiusSpeed;

	    if (radius < 0)
	        radius = 0;

	    if (Input.GetKey(KeyCode.A))
	        rotationAngle += Time.deltaTime * rotationSpeed;
	    else if (Input.GetKey(KeyCode.D))
	        rotationAngle -= Time.deltaTime * rotationSpeed;

	    if (Input.GetKey(KeyCode.Q))
	        angle += Time.deltaTime * angleSpeed;
	    else if (Input.GetKey(KeyCode.E))
	        angle -= Time.deltaTime * angleSpeed;

        angle = Mathf.Clamp(angle, 0, 360);

        if (Input.GetKey(KeyCode.X))
            strenght += Time.deltaTime * strenghtSpeed;
        else if (Input.GetKey(KeyCode.Z))
            strenght -= Time.deltaTime * strenghtSpeed;

        strenght = Mathf.Clamp(strenght, 0.1f, strenghtMax);

	    Color a = centerColor;
	    a.a = 0.34f + strenght/strenghtMax*0.66f;
	    centerColor = a;

	    if (vertices == null || vertices.Length != meshAngleSeparation + 1)
	    {
	        vertices = new Vector3[meshAngleSeparation + 2];
	        colors = new Color[meshAngleSeparation + 2];
	        triangles = new int[meshAngleSeparation * 3 + 3];
            //uvs = new Vector2[meshAngleSeparation + 2];
	    }

	    Mesh mesh = meshFilter.mesh;
	    if (mesh == null)
	    {
	        mesh = new Mesh();
	        meshFilter.mesh = mesh;
	    }

	    vertices[0] = Vector3.zero;
	    colors[0] = centerColor;
        //uvs[0] = new Vector2(0, 0.5f);
	    for (int i = 1; i < meshAngleSeparation + 2; i++)
	    {
	        float rotVal = (angle / meshAngleSeparation) * (i - 1) + rotationAngle;
	        float x = Mathf.Cos(Mathf.Deg2Rad * rotVal);
	        float y = Mathf.Sin(Mathf.Deg2Rad * rotVal);
	        float xRad = radius * x;
	        float yRad = radius * y;
	        tempV3.x = xRad;
	        tempV3.y = yRad;
	        tempV3.z = 0;
	        vertices[i] = tempV3;
	        colors[i] = externalColor;
            //uvs[i] = new Vector2(1, 0.5f);
	    }

	    int index = 0;
	    for (int i = 0; i < meshAngleSeparation * 3; i += 3)
	    {
	        triangles[i] = 0;
	        triangles[i + 1] = index + 2;
	        triangles[i + 2] = index + 1;
	        index++;
	    }

        mesh.Clear();
	    mesh.vertices = vertices;
	    mesh.triangles = triangles;
	    mesh.colors = colors;
        //mesh.uv = uvs;
    }

    public void Show(bool b)
    {
        meshFilter.gameObject.SetActive(b);
    }
}
