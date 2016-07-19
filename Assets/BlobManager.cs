using UnityEngine;
using System.Collections;

public class BlobManager : MonoBehaviour {

    //Outer loop of the blob : Assign In Ediot
    public Transform[] outerLoop;
    //Center of the blob : Assign in editor
    public Transform center;

    MeshFilter _mf ;

    private Mesh blobMesh;

	// Use this for initialization
	void Start () {
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

        // 2x Since adding midpoinst, +1 for the center
        int numVert = 2*outerLoop.Length + 1;

        //outer + inner tris
        int numTris = 2 * outerLoop.Length;

        Vector3[] _vertices = new Vector3[numVert];
        int[] _triangles = new int[3 * numTris];

        //Start Populating vertices

        //Add Last Midpoint
        Vector3 lastMid =(outerLoop[0].position +  outerLoop[outerLoop.Length - 1].position )/2.0f;
        //First is midpoint for first and last
        _vertices[0] = lastMid;

        //Last vertex is the center of the blob
        _vertices[_vertices.Length - 1] = center.position;
        int vertCtr = 1;
        int triCtr = 0;
        int numOuterVerts = numVert - 1;
        for( int it_outer = 0 ; it_outer < outerLoop.Length ; it_outer++){
            Vector3 curr = outerLoop[it_outer].position;
            Vector3 next = outerLoop[(it_outer + 1)% outerLoop.Length].position;

            //Debug.Log("Curr"+next);

            _vertices[vertCtr] = curr; vertCtr = (vertCtr+1) ;


            if(it_outer !=(outerLoop.Length-1) ){
                Vector3 mid = (curr + next)/2.0f; 
                _vertices[vertCtr] = mid; vertCtr = (vertCtr+1) ;
            }


            //Adding Outer loop triangle
            int first = vertCtr -3;
            int middle = vertCtr - 2;
            int last = vertCtr - 1;
            if( it_outer ==(outerLoop.Length-1)){
                first = vertCtr -2;
                middle = vertCtr - 1;
                last = 0;
            }
            _triangles[triCtr] = first;    ++triCtr;
            _triangles[triCtr] = middle; ++triCtr;
            _triangles[triCtr] = last; ++triCtr;

            //Adding inner fill triangle
            _triangles[triCtr] = first;    triCtr++;
            _triangles[triCtr] = last; triCtr++;
            _triangles[triCtr] = _vertices.Length - 1; triCtr++;
           
        }
        blobMesh.vertices = _vertices;
        blobMesh.triangles = _triangles;
        blobMesh.RecalculateNormals();
        _mf.mesh = blobMesh;


        
      
	}


	
	// Update is called once per frame
	void Update () {
        int vertCt = 1;
        Vector3[] _vertices = _mf.mesh.vertices;
        for( int it_outer = 0 ; it_outer < outerLoop.Length ; it_outer++){
            Vector3 curr = outerLoop[it_outer].position;
            Vector3 next = outerLoop[(it_outer + 1)% outerLoop.Length].position;
            Vector3 mid = (curr + next)/2.0f;

            _vertices[vertCt] = curr;   vertCt++;

            if ( it_outer !=(outerLoop.Length-1) ){
                _vertices[vertCt] = mid;    vertCt++;
            }

        }
        
        //Add Last Midpoint
        Vector3 lastMid =(outerLoop[0].position +  outerLoop[outerLoop.Length - 1].position )/2;
        //First is midpoint for first and last
        _vertices[0] = lastMid;
        
        //Last vertex is the center of the blob
        _vertices[_vertices.Length - 1] = center.position;
        _mf.mesh.vertices = _vertices;


	}
}
