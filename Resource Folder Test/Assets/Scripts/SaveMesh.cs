using UnityEngine;
using System.Collections;

public class SaveMesh : MonoBehaviour {
    string fileName= "SerializedMesh.data";
    bool saveTangents= false;

    void  Start ()
    {
        Mesh inputMesh= GetComponent<MeshFilter>().mesh;
        string fullFileName= Application.dataPath + "/" + fileName;
        MeshSerializer.WriteMeshToFile(inputMesh, fullFileName, saveTangents);
        Debug.LogError("Saved " + name + " mesh to " + fullFileName );
    }
}