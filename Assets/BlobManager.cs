using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class BlobManager : MonoBehaviour {

    //Outer loop of the blob : Assign In Ediot
    public Transform[] outerLoop;
    //Center of the blob : Assign in editor
    public Transform center;


    MeshFilter _mf ;

    private Mesh blobMesh;

    Vector2[] refUVs = { new Vector2(0,0), new Vector2(0,0.5f), new Vector2(1,1)};
	// Use this for initialization
	void Start () {
        transform.position = Vector3.zero;

	    //Creating and adding a Mesh
        blobMesh = new Mesh();

        _mf = GetComponent<MeshFilter>();
        //If not added in editor already
        if(_mf == null){
            this.gameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer _mr = GetComponent<MeshRenderer>();
        if( _mr == null ){
            this.gameObject.AddComponent<MeshRenderer>();
        }

        int numVerts = 3*outerLoop.Length ;
        int numTris = outerLoop.Length*3;
        List<Vector3> _vertices = new List<Vector3>(numVerts);
        Vector2[] _uv = new Vector2[numVerts];
        List<int> _triangles = new List<int>(numTris);

      
     

        //Subdivide outer loop 
        int uvCtr = 0;
        for( int it_outer = 0 ; it_outer < outerLoop.Length ; it_outer++){
            Vector3 curr = outerLoop[myMod( it_outer, outerLoop.Length)].position;
            Vector3 next = outerLoop[myMod(it_outer + 1, outerLoop.Length)].position;

            _vertices.Add(curr);
            Vector3 mid = (curr + next)/2.0f;
            _vertices.Add(mid);
            _vertices.Add(mid);
           
           
        }
        //_vertices.Add(center.position);

        //Generate triangles from computed vertices
        int center_i = _vertices.Count - 1;
        for( int it_outer = 0 ; it_outer < outerLoop.Length ; it_outer++){
            int p1_i = 3 * it_outer;
            int p2_i = myMod(3 * it_outer + 1, _vertices.Count );
            int p3_i = myMod(3 * it_outer - 1, _vertices.Count );

            _triangles.Add(p3_i);
            _triangles.Add(p1_i);
            _triangles.Add(p2_i);


            Vector2  offset = new Vector2(it_outer, it_outer);
            _uv[p3_i] = refUVs[0] ;
            _uv[p1_i] = refUVs[1] ;
            _uv[p2_i] = refUVs[2] ;

            //_triangles.Add(p2_i);
            //_triangles.Add(center_i);
            //_triangles.Add(p3_i);

        }
        blobMesh.vertices = _vertices.ToArray();
        blobMesh.uv = _uv;
        blobMesh.triangles = _triangles.ToArray();
        blobMesh.RecalculateNormals();
        _mf.mesh = blobMesh;

        //Debug.Break();
        
      
	}

    int myMod(int p1,int p2){
        return (p1%p2 + p2)%p2;
    }


	
	// Update is called once per frame
	void FixedUpdate () {

        Vector3[] _vertices = _mf.mesh.vertices;
        for( int it_outer=0; it_outer< outerLoop.Length; it_outer++){
            Vector3 curr = outerLoop[it_outer].position;
            Vector3 next = outerLoop[(it_outer + 1)% outerLoop.Length].position;

            _vertices[3*it_outer] = curr;
            Vector3 mid = (curr + next)/2.0f;
            _vertices[3*it_outer + 1 ] = mid;
            _vertices[3*it_outer + 2] = mid;
        }
        //_vertices[_vertices.Length - 1] = center.position;
        _mf.mesh.vertices = _vertices;


	}
}
