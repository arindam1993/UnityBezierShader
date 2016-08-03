using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class BlobManager : MonoBehaviour {

    //Outer loop of the blob : Assign In Ediot
    public Transform[] outerLoop;
    //Center of the blob : Assign in editor
    public Transform center;
    //Adding some force internally to keep the blob "infated"
    public float  inflationIntensity;
    //Offset the vertices outward slightly so they are closer to the outer edge
    public float offsetFactor;

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

        int numVerts = 3*outerLoop.Length + 1;
        int numTris = 2*outerLoop.Length*3;
        List<Vector3> _vertices = new List<Vector3>(numVerts);
        Vector2[] _uv = new Vector2[numVerts];
        List<int> _triangles = new List<int>(numTris);

      
     

        //Subdivide outer loop 
        int uvCtr = 0;
        for( int it_outer = 0 ; it_outer < outerLoop.Length ; it_outer++){
            //Circluar indexing on the outer loop
            Vector3 curr = _offset(outerLoop[myMod( it_outer, outerLoop.Length)].position);
            Vector3 next = _offset(outerLoop[myMod(it_outer + 1, outerLoop.Length)].position);

            _vertices.Add(curr);
            //Calculate and add double midpoints
            Vector3 mid = (curr + next)/2.0f;
            _vertices.Add(mid);
            _vertices.Add(mid);
           
           
        }
        //Add the central vertex at the end
        _vertices.Add(center.position);

        //Generate triangles from computed vertices
        int center_i = _vertices.Count - 1;
        for( int it_outer = 0 ; it_outer < outerLoop.Length ; it_outer++){
            //the tips of each traingle, because of the way we populated the array will be at indices 0,3,6, ....
            int p1_i = 3 * it_outer;
            //Circluat index to get the previous and next ertex
            int p2_i = myMod(3 * it_outer + 1, _vertices.Count - 1);
            int p3_i = myMod(3 * it_outer - 1, _vertices.Count  -1);

            //Add the triangles
            _triangles.Add(p3_i);
            _triangles.Add(p1_i);
            _triangles.Add(p2_i);

            //Add the central triangles
            _triangles.Add(p2_i);
            _triangles.Add(center_i);
            _triangles.Add(p3_i);


            //Set up UV's for the outer triangles
            _uv[p3_i] = refUVs[0] ;
            _uv[p1_i] = refUVs[1] ;
            _uv[p2_i] = refUVs[2] ;

         

        }
        blobMesh.vertices = _vertices.ToArray();
        blobMesh.uv = _uv;
        blobMesh.triangles = _triangles.ToArray();
        blobMesh.RecalculateNormals();
        _mf.mesh = blobMesh;

        Debug.Break();
        
      
	}

    //Circular indexing utility function
    int myMod(int p1,int p2){
        return (p1%p2 + p2)%p2;
    }

    //Utility vertex offsetting function
    Vector3 _offset(Vector3 pt){
        return pt + (pt-center.position)*offsetFactor;
    }

	
	// Update is called once per frame
	void Update () {

        Vector3[] _vertices = _mf.mesh.vertices;
        //Update all the vertex positions every frame
        for( int it_outer=0; it_outer< outerLoop.Length; it_outer++){


            Vector3 curr = _offset(outerLoop[it_outer].position);
            Vector3 next = _offset(outerLoop[(it_outer + 1)% outerLoop.Length].position);

            _vertices[3*it_outer] = curr;
            Vector3 mid = (curr + next)/2.0f;
            _vertices[3*it_outer + 1 ] = mid;
            _vertices[3*it_outer + 2] = mid;

            Rigidbody2D _rbd = outerLoop[it_outer].gameObject.GetComponent<Rigidbody2D>();
            Vector3 forceVec = curr - center.position;

            //Force to inflate the blob;
            _rbd.AddForce(forceVec * inflationIntensity);


        }
        _vertices[_vertices.Length - 1] = center.position;
        _mf.mesh.vertices = _vertices;


	}
}
