using UnityEngine;
using System.Collections;
using System.IO;

public class MeshSerializer : MonoBehaviour {

// A simple mesh saving/loading functionality.
// This is an utility script, you don't need to add it to any objects.
// See SaveMeshForWeb and LoadMeshFromWeb for example of use.
//
// Uses a custom binary format:
//
//    2 bytes vertex count
//    2 bytes triangle count
//    1 bytes vertex format (bits: 0=vertices, 1=normals, 2=tangents, 3=uvs)
//
//    After that come vertex component arrays, each optional except for positions.
//    Which ones are present depends on vertex format:
//        Positions
//            Bounding box is before the array (xmin,xmax,ymin,ymax,zmin,zmax)
//            Then each vertex component is 2 byte unsigned short, interpolated between the bound axis
//        Normals
//            One byte per component
//        Tangents
//            One byte per component
//        UVs (8 bytes/vertex - 2 floats)
//            Bounding box is before the array (xmin,xmax,ymin,ymax)
//            Then each UV component is 2 byte unsigned short, interpolated between the bound axis
//
//    Finally the triangle indices array: 6 bytes per triangle (3 unsigned short indices)


// Reads mesh from an array of bytes. Can return null
// if the bytes seem invalid.
    public static Mesh ReadMesh (  byte[] bytes   )
    {
        if( bytes == null || bytes.Length < 5 )
        {
            Debug.Log("Invalid mesh file!");
            return null;
        }

        BinaryReader buf = new BinaryReader(new MemoryStream(bytes));
    
        // read header
        int vertCount= buf.ReadUInt16();
        int triCount = buf.ReadUInt16();
        int format = buf.ReadByte();
    
        // sanity check
        if (vertCount < 0 || vertCount > 64000)
        {
            Debug.Log("Invalid vertex count in the mesh data!");
            return null;
        }
        if (triCount < 0 || triCount > 64000)
        {
            Debug.Log("Invalid triangle count in the mesh data!");
            return null;
        }
        if (format < 1 || (format&1) == 0 || format > 15)
        {
            Debug.Log("Invalid vertex format in the mesh data!");
            return null;
        }
    
        Mesh mesh= new Mesh();
        int i;
    
        // positions
        Vector3[] verts = new Vector3[vertCount];
        ReadVector3Array16bit (verts, buf);
        mesh.vertices = verts;
    
        if( (format & 2) != 0 ) // have normals
        {
            Vector3[] normals = new Vector3[vertCount];
            ReadVector3ArrayBytes (normals, buf);
            mesh.normals = normals;
        }
    
        if( (format & 4) != 0) // have tangents
        {
            Vector4[] tangents = new Vector4[vertCount];
            ReadVector4ArrayBytes (tangents, buf);
            mesh.tangents = tangents;
        }
    
        if( (format & 8) != 0 ) // have UVs
        {
            Vector2[] uvs = new Vector2[vertCount];
            ReadVector2Array16bit (uvs, buf);
            mesh.uv = uvs;
        }
    
        // triangle indices
        int[] tris = new int[triCount * 3];
        for( i = 0; i < triCount; ++i )
        {
            tris[i*3+0] = buf.ReadUInt16();
            tris[i*3+1] = buf.ReadUInt16();
            tris[i*3+2] = buf.ReadUInt16();
        }
        mesh.triangles = tris;
    
        buf.Close();
    
        return mesh;
    }

    public static void ReadVector3Array16bit ( Vector3[] arr ,   BinaryReader buf  ){
        int n= arr.Length;
        if (n == 0)
            return;
        
        // Read bounding box
        Vector3 bmin;
        Vector3 bmax;
        bmin.x = buf.ReadSingle ();
        bmax.x = buf.ReadSingle ();
        bmin.y = buf.ReadSingle ();
        bmax.y = buf.ReadSingle ();
        bmin.z = buf.ReadSingle ();
        bmax.z = buf.ReadSingle ();
    
        // Decode vectors as 16 bit integer components between the bounds
        for (int i= 0; i < n; ++i) {
            System.UInt16 ix = buf.ReadUInt16 ();
            System.UInt16 iy = buf.ReadUInt16 ();
            System.UInt16 iz = buf.ReadUInt16 ();
            float xx = ix / 65535.0f * (bmax.x - bmin.x) + bmin.x;
            float yy = iy / 65535.0f * (bmax.y - bmin.y) + bmin.y;
            float zz = iz / 65535.0f * (bmax.z - bmin.z) + bmin.z;
            arr[i] = new Vector3 (xx,yy,zz);
        }
    }

    public static void WriteVector3Array16bit ( Vector3[] arr ,   BinaryWriter buf  ){
        if (arr.Length == 0)
            return;
    
        // Calculate bounding box of the array
        Bounds bounds = new Bounds(arr[0], new Vector3(0.001f, 0.001f, 0.001f));
        foreach(var v in arr)
            bounds.Encapsulate (v);
        
        // Write bounds to stream
        Vector3 bmin= bounds.min;
        Vector3 bmax = bounds.max;
        buf.Write (bmin.x);
        buf.Write (bmax.x);
        buf.Write (bmin.y);
        buf.Write (bmax.y);
        buf.Write (bmin.z);
        buf.Write (bmax.z);
    
        // Encode vectors as 16 bit integer components between the bounds
        foreach(var v in arr) {
            float xx = Mathf.Clamp ((v.x - bmin.x) / (bmax.x - bmin.x) * 65535.0f, 0.0f, 65535.0f);
            float yy = Mathf.Clamp((v.y - bmin.y) / (bmax.y - bmin.y) * 65535.0f, 0.0f, 65535.0f);
            float zz = Mathf.Clamp((v.z - bmin.z) / (bmax.z - bmin.z) * 65535.0f, 0.0f, 65535.0f);
            System.UInt16 ix = System.Convert.ToUInt16(xx);
            System.UInt16 iy = System.Convert.ToUInt16(yy);
            System.UInt16 iz = System.Convert.ToUInt16(zz);
            buf.Write (ix);
            buf.Write (iy);
            buf.Write (iz);
        }
    }


    public static void ReadVector2Array16bit ( Vector2[] arr ,   BinaryReader buf  ){
        int n= arr.Length;
        if (n == 0)
            return;
        
        // Read bounding box
        Vector2 bmin;
        Vector2 bmax;
        bmin.x = buf.ReadSingle ();
        bmax.x = buf.ReadSingle ();
        bmin.y = buf.ReadSingle ();
        bmax.y = buf.ReadSingle ();
    
        // Decode vectors as 16 bit integer components between the bounds
        for (int i= 0; i < n; ++i) {
            System.UInt16 ix = buf.ReadUInt16 ();
            System.UInt16 iy = buf.ReadUInt16 ();
            float xx = ix / 65535.0f * (bmax.x - bmin.x) + bmin.x;
            float yy = iy / 65535.0f * (bmax.y - bmin.y) + bmin.y;
            arr[i] = new Vector2 (xx,yy);
        }
    }

    public static void WriteVector2Array16bit(Vector2[] arr, BinaryWriter buf)
    {
        if (arr.Length == 0)
            return;
    
        // Calculate bounding box of the array
        Vector2 bmin = arr[0] - new Vector2(0.001f,0.001f);
        Vector2 bmax = arr[0] + new Vector2(0.001f,0.001f);
        foreach(var v in arr) {
            bmin.x = Mathf.Min (bmin.x, v.x);
            bmin.y = Mathf.Min (bmin.y, v.y);
            bmax.x = Mathf.Max (bmax.x, v.x);
            bmax.y = Mathf.Max (bmax.y, v.y);
        }
        
        // Write bounds to stream
        buf.Write (bmin.x);
        buf.Write (bmax.x);
        buf.Write (bmin.y);
        buf.Write (bmax.y);
    
        // Encode vectors as 16 bit integer components between the bounds
        foreach(var v in arr) {
            float xx= (v.x - bmin.x) / (bmax.x - bmin.x) * 65535.0f;
            float yy= (v.y - bmin.y) / (bmax.y - bmin.y) * 65535.0f;
            System.UInt16 ix = System.Convert.ToUInt16(xx);
            System.UInt16 iy = System.Convert.ToUInt16(yy);
            buf.Write (ix);
            buf.Write (iy);
        }
    }

    public static void ReadVector3ArrayBytes(Vector3[] arr, BinaryReader buf)
    {
        // Decode vectors as 8 bit integers components in -1.0f .. 1.0f range
        int n = arr.Length;
        for (int i = 0; i < n; ++i) {
            byte ix = buf.ReadByte ();
            byte iy = buf.ReadByte ();
            byte iz = buf.ReadByte ();
            float xx = (ix - 128.0f) / 127.0f;
            float yy = (iy - 128.0f) / 127.0f;
            float zz = (iz - 128.0f) / 127.0f;
            arr[i] = new Vector3(xx,yy,zz);
        }
    }

    public static void WriteVector3ArrayBytes(Vector3[] arr, BinaryWriter buf)
    {
        // Encode vectors as 8 bit integers components in -1.0f .. 1.0f range
        foreach(var v in arr) {
            byte ix = System.Convert.ToByte(Mathf.Clamp (v.x * 127.0f + 128.0f, 0.0f, 255.0f));
            byte iy = System.Convert.ToByte(Mathf.Clamp (v.y * 127.0f + 128.0f, 0.0f, 255.0f));
            byte iz = System.Convert.ToByte(Mathf.Clamp (v.z * 127.0f + 128.0f, 0.0f, 255.0f));
            buf.Write (ix);
            buf.Write (iy);
            buf.Write (iz);
        }
    }

    public static void ReadVector4ArrayBytes(Vector4[] arr, BinaryReader buf)
    {
        // Decode vectors as 8 bit integers components in -1.0f .. 1.0f range
        int n= arr.Length;
        for (int i = 0; i < n; ++i) {
            byte ix = buf.ReadByte ();
            byte iy = buf.ReadByte ();
            byte iz = buf.ReadByte ();
            byte iw = buf.ReadByte ();
            float xx = (ix - 128.0f) / 127.0f;
            float yy = (iy - 128.0f) / 127.0f;
            float zz = (iz - 128.0f) / 127.0f;
            float ww = (iw - 128.0f) / 127.0f;
            arr[i] = new Vector4(xx,yy,zz,ww);
        }
    }

    public static void WriteVector4ArrayBytes(Vector4[] arr, BinaryWriter buf)
    {
        // Encode vectors as 8 bit integers components in -1.0f .. 1.0f range
        foreach(var v in arr) {
            byte ix = System.Convert.ToByte(Mathf.Clamp (v.x * 127.0f + 128.0f, 0.0f, 255.0f));
            byte iy = System.Convert.ToByte(Mathf.Clamp (v.y * 127.0f + 128.0f, 0.0f, 255.0f));
            byte iz = System.Convert.ToByte(Mathf.Clamp (v.z * 127.0f + 128.0f, 0.0f, 255.0f));
            byte iw = System.Convert.ToByte(Mathf.Clamp (v.w * 127.0f + 128.0f, 0.0f, 255.0f));
            buf.Write (ix);
            buf.Write (iy);
            buf.Write (iz);
            buf.Write (iw);
        }
    }

    // Writes mesh to an array of bytes.
    public static byte[] WriteMesh(Mesh mesh, bool saveTangents)
    {
        if( !mesh )
        {
            print( "No mesh given!" );
            return null;
        }
    
        Vector3[] verts= mesh.vertices;
        Vector3[] normals= mesh.normals;
        Vector4[] tangents= mesh.tangents;
        Vector2[] uvs= mesh.uv;    
        int[] tris= mesh.triangles;
    
        // figure out vertex format
        byte format = 1;
        if( normals.Length > 0 )
            format |= 2;
        if( saveTangents && tangents.Length > 0 )
            format |= 4;
        if( uvs.Length > 0 )
            format |= 8;

        MemoryStream stream = new MemoryStream();
        BinaryWriter buf = new BinaryWriter(stream);
    
        // write header
        System.UInt16 vertCount = System.Convert.ToUInt16(verts.Length);
        System.UInt16 triCount = System.Convert.ToUInt16(tris.Length / 3);
        buf.Write( vertCount );
        buf.Write( triCount );
        buf.Write( format );
        // vertex components
        WriteVector3Array16bit (verts, buf);
        WriteVector3ArrayBytes (normals, buf);
        if (saveTangents)
            WriteVector4ArrayBytes (tangents, buf);
        WriteVector2Array16bit (uvs, buf);
        // triangle indices
        foreach( var idx in tris ) {
            System.UInt16 idx16 = System.Convert.ToUInt16(idx);
            buf.Write( idx16 );
        }
        buf.Close();
    
        return stream.ToArray();
    }


    // Writes mesh to a local file, for loading with WWW interface later.
    public static void WriteMeshToFile(Mesh mesh, string name, bool saveTangents)
    {
         // Write mesh to regular bytes
        byte[] bytes= WriteMesh( mesh, saveTangents );
    
        // Write to file
        FileStream fs = new FileStream(name, FileMode.Create);
        fs.Write( bytes, 0, bytes.Length );
        fs.Close();
    }
}