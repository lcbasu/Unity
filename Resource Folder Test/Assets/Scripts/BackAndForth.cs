using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Text;
using System.IO;


public class BackAndForth : MonoBehaviour {
    public float delta = 1.5f;  // Amount to move left and right from the start point
    public float speed = 2.0f;
    private Vector3 startPos;

    public string FileName = "";
    private TextAsset asset = null;
    private StreamWriter writer = null;

    void Start()
    {
        startPos = transform.position;


        FileName = "test_file";

        //Load(Application.dataPath + "\\test_file.txt");

        // Load a material from resource folder

        Material mat = Resources.Load("Materials/test_material") as Material;

        if (mat != null)
        {
            Debug.LogError("material loaded");
        }
        else
        {
            Debug.LogError("material NOT loaded");
        }

#if UNITY_EDITOR
        // Print the path of the created asset
        Debug.LogError(AssetDatabase.GetAssetPath(mat));

        // Create asset
        Mesh mesh_cylinder = CreatePrimitiveMesh("CYLINDER");
        AssetDatabase.CreateAsset(mesh_cylinder, "Assets/Resources/" + mesh_cylinder.name + ".asset");

        // Load the created asset
        Mesh mesh = Resources.Load(mesh_cylinder.name) as Mesh;
        if (mesh != null)
        {
            Debug.LogError("mesh loaded");
        }
        else
        {
            Debug.LogError("mesh NOT loaded");
        }
        // Print the path of the loaded asset
        Debug.LogError(AssetDatabase.GetAssetPath(mesh));
#endif
        // Append string

        //AppendString("Hello world!");
    }

    void Update()
    {
        Vector3 v = startPos;
        v.x += delta * Mathf.Sin(Time.time * speed);
        transform.position = v;
    }


    Mesh CreatePrimitiveMesh(string meshname)
    {
        //Mesh mesh  = new Mesh();
        GameObject tempobject;
        switch (meshname)
        {
            default:
                tempobject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case "CUBE":
                tempobject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case "CYLINDER":
                tempobject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
            case "SPHERE":
                tempobject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
        }
        Mesh mesh = tempobject.GetComponent<MeshFilter>().mesh;
        mesh.name = meshname;

        tempobject.SetActive(false);
        return mesh;
    }

    //void AppendString(string FileName)
    //{
    //    StreamWriter sw = new StreamWriter(FileName);
    //    // Add some text to the file.
    //    sw.Write("This is the ");
    //    sw.WriteLine("header for the file.");
    //    sw.WriteLine("-------------------");
    //    // Arbitrary objects can also be written to the file.
    //    sw.Write("The date is: ");
    //    sw.WriteLine(DateTime.Now);
    //    sw.Close();
    //}

    bool Load(string fileName)
    {
        string line;
        StreamReader theReader = new StreamReader(fileName, Encoding.Default);
        using (theReader)
        {
            do
            {
                line = theReader.ReadLine();
                if (line != null)
                {
                    Debug.LogError(line);

                }
            }
            while (line != null);
            theReader.Close();
            return true;
        }
    }
}
