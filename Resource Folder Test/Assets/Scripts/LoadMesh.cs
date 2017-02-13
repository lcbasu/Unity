using UnityEngine;
using System.Collections;
using System.IO;

public class LoadMesh : MonoBehaviour {

    string fileName = "SerializedMesh.asset";

    bool meshLoaded = false;

    void  Update ()
    {
        string path = "Assets/Resources/Meshes/";
        Mesh mesh = MeshSerializer.ReadMesh(File.ReadAllBytes(path + fileName));
        if (mesh && !meshLoaded)
        {
            Debug.LogError("Mesh loaded");
            meshLoaded = true;
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (!meshFilter)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
                gameObject.AddComponent<MeshRenderer>();
                GetComponent<Renderer>().material.color = Color.red;
            }
            meshFilter.mesh = mesh;
        }
        else if (!mesh)
        {
            Debug.LogError("Failed to load mesh");
            return;
        }
    }

}