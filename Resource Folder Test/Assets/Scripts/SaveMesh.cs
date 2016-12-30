using UnityEngine;
using System.Collections;
using System.IO;

public class SaveMesh : MonoBehaviour {
    string fileName = "SerializedMesh.asset";
    bool saveTangents= false;

    void  Start ()
    {
        string path = Application.dataPath + "/Resources/Meshes/";
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }

        Mesh inputMesh= GetComponent<MeshFilter>().mesh;
        string fullFileName = path + fileName;
        MeshSerializer.WriteMeshToFile(inputMesh, fullFileName, saveTangents);
        Debug.LogError("Saved " + name + " mesh to " + fullFileName );
    }
}