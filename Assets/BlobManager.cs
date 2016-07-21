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

        int numVerts = 2*outerLoop.Length +1;
        int numTris = 2*outerLoop.Length*3;
        List<Vector3> _vertices = new List<Vector3>(numVerts);
        List<int> _triangles = new List<int>(numTris);

      
     

        //Subdivide outer loop 
        for( int it_outer = 0 ; it_outer < outerLoop.Length ; it_outer++){
            Vector3 curr = outerLoop[myMod( it_outer, outerLoop.Length)].position;
            Vector3 next = outerLoop[myMod(it_outer + 1, outerLoop.Length)].position;

            _vertices.Add(curr);
            _vertices.Add((curr + next)/2.0f);
           
        }
        _vertices.Add(center.position);
        //Generate triangles from computed vertices
        int center_i = _vertices.Count - 1;
        for( int it_outer = 0 ; it_outer < outerLoop.Length ; it_outer++){
            int p1_i = 2 * it_outer;
            int p2_i = myMod(2 * it_outer + 1, _vertices.Count - 1);
            int p3_i = myMod(2 * it_outer - 1, _vertices.Count - 1);

            _triangles.Add(p1_i);
            _triangles.Add(p2_i);
            _triangles.Add(p3_i);

            _triangles.Add(p2_i);
            _triangles.Add(center_i);
            _triangles.Add(p3_i);

        }
        blobMesh.vertices = _vertices.ToArray();
        blobMesh.triangles = _triangles.ToArray();
        blobMesh.RecalculateNormals();
        _mf.mesh = blobMesh;


        
      
	}

    int myMod(int p1,int p2){
        return (p1%p2 + p2)%p2;
    }


	
	// Update is called once per frame
	void Update () {

        Vector3[] _vertices = _mf.mesh.vertices;
        for( int it_outer=0; it_outer< outerLoop.Length; it_outer++){
            Vector3 curr = outerLoop[it_outer].position;
            Vector3 next = outerLoop[(it_outer + 1)% outerLoop.Length].position;

            _vertices[2*it_outer] = curr;
            _vertices[2*it_outer + 1] = (curr + next)/2.0f;
        }
        _vertices[_vertices.Length - 1] = center.position;
        _mf.mesh.vertices = _vertices;


	}
}
