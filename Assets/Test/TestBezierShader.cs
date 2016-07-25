using UnityEngine;
using System.Collections;

public class TestBezierShader : MonoBehaviour {

    public Transform[] cntrlPts;


    MeshFilter mf;

	void Start () {

        Mesh mesh = new Mesh();
        mf = GetComponent<MeshFilter>();
        Vector3[] _verts = new Vector3[3];
        for ( int i = 0 ; i < 3 ; i++){
            _verts[i] = cntrlPts[i].position;
            Debug.Log(_verts[i]);
        }

        int[] _tris =  {0, 1, 2};
        Vector2[] _uvs =  { new Vector2(0,0), new Vector2(0, 0.5f), new Vector2(1,1)};


        mesh.vertices = _verts;
        mesh.triangles = _tris;
        mesh.uv = _uvs;
        mesh.RecalculateNormals();
        mf.mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3[] _verts = mf.mesh.vertices;
        for ( int i = 0 ; i < 3 ; i++){
            _verts[i] = cntrlPts[i].position;
            //Debug.Log(_verts[i]);
        }
        mf.mesh.vertices = _verts;
	
	}
}
