using UnityEngine;
using System.Collections;
using System.IO;

public class LoadMesh : MonoBehaviour {

    string fileName = "SerializedMesh.data";

    void  Start ()
    {
        Mesh mesh = MeshSerializer.ReadMesh(File.ReadAllBytes(Application.dataPath + "/" + fileName));
        if (!mesh)
        {
            Debug.Log("Failed to load mesh");
            return;
        }
        Debug.Log("Mesh loaded");
    
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if( !meshFilter ) {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            GetComponent<Renderer>().material.color = Color.red;
        }
        meshFilter.mesh = mesh;
    }

}