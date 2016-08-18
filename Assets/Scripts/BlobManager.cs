using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class BlobManager : MonoBehaviour {

    public GameObject maskGO;

    public GameObject meshGO;

    //Outer loop of the blob : Assign In Ediot
    public Transform[] outerLoop;
    //Center of the blob : Assign in editor
    public Transform center;
    //Adding some force internally to keep the blob "infated"
    public float  inflationIntensity;
    //Offset the vertices outward slightly so they are closer to the outer edge
    public float offsetFactor;

    public Material maskMat;
    public Material meshMat;

    MeshFilter _maskMf;
    MeshFilter _meshMf;

    Vector2[] refUVs = { new Vector2(0,0), new Vector2(0,0.5f), new Vector2(1,1)};

	// Use this for initialization
	void Start () {
        transform.position = Vector3.zero;
        
        _initMask();
        _initMesh();

        Debug.Break();
      
	}

    #region MESH_METHODS

    private void _initMesh()
    {

        
        MeshRenderer _mr;
        _initRequiredCmponents(meshGO, out _meshMf,out _mr);
        meshGO.GetComponent<Renderer>().sharedMaterial = meshMat;

        int numVerts = outerLoop.Length + 1;
        int numTris = 3 * outerLoop.Length;

        Vector3[] verts = new Vector3[numVerts];
        List<int> tris = new List<int>(numTris);
        Vector2[] uvs = new Vector2[numVerts];

        verts[numVerts - 1] = center.position;

        float phi = Mathf.PI * 2 / outerLoop.Length;

        uvs[numVerts - 1] = new Vector2(0.5f, 0.5f);
        for (int it_out = 0; it_out < outerLoop.Length; it_out++)
        {
            verts[it_out] = _offset(outerLoop[it_out].position);

            tris.Add(it_out);
            tris.Add(myMod(it_out + 1, outerLoop.Length));
            tris.Add(numVerts - 1);


            float theta = phi * it_out;
            float x = -1 * 0.5f * Mathf.Cos(theta) + 0.5f;
            float y =   0.5f * Mathf.Sin(theta) + 0.5f;

            uvs[it_out] = new Vector2(x, y);

            
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs;

        _meshMf.mesh = mesh;



    }
    #endregion

    #region MASK_METHODS

    private void _initMask()
    {
        //Creating and adding a Mesh
        Mesh blobMesh = new Mesh();
        MeshRenderer _mr;
        _initRequiredCmponents(maskGO,out _maskMf,out _mr);
     


        maskGO.GetComponent<Renderer>().sharedMaterial = maskMat;

        int numVerts = 3 * outerLoop.Length ;
        int numTris =  outerLoop.Length * 3;
        List<Vector3> _vertices = new List<Vector3>(numVerts);
        Vector2[] _uv = new Vector2[numVerts];
        List<int> _triangles = new List<int>(numTris);




        //Subdivide outer loop 
        for (int it_outer = 0; it_outer < outerLoop.Length; it_outer++)
        {
            //Circluar indexing on the outer loop
            Vector3 curr = _offset(outerLoop[myMod(it_outer, outerLoop.Length)].position);
            Vector3 next = _offset(outerLoop[myMod(it_outer + 1, outerLoop.Length)].position);

            _vertices.Add(curr);
            //Calculate and add double midpoints
            Vector3 mid = (curr + next) / 2.0f;
            _vertices.Add(mid);
            _vertices.Add(mid);


        }
   

        //Generate triangles from computed vertices
        for (int it_outer = 0; it_outer < outerLoop.Length; it_outer++)
        {
            //the tips of each traingle, because of the way we populated the array will be at indices 0,3,6, ....
            int p1_i = 3 * it_outer;
            //Circluat index to get the previous and next ertex
            int p2_i = myMod(3 * it_outer + 1, _vertices.Count );
            int p3_i = myMod(3 * it_outer - 1, _vertices.Count );

            //Add the triangles
            _triangles.Add(p3_i);
            _triangles.Add(p1_i);
            _triangles.Add(p2_i);


            //Set up UV's for the outer triangles
            _uv[p3_i] = refUVs[0];
            _uv[p1_i] = refUVs[1];
            _uv[p2_i] = refUVs[2];



        }
        blobMesh.vertices = _vertices.ToArray();
        blobMesh.uv = _uv;
        blobMesh.triangles = _triangles.ToArray();
        blobMesh.RecalculateNormals();
        _maskMf.mesh = blobMesh;

    }

    private void _updateMeshes()
    {

        Vector3[] _maskVerts = _maskMf.mesh.vertices;
        Vector3[] _meshVerts = _meshMf.mesh.vertices;
        //Update all the vertex positions every frame
        for (int it_outer = 0; it_outer < outerLoop.Length; it_outer++)
        {


            Vector3 curr = _offset(outerLoop[it_outer].position);
            Vector3 next = _offset(outerLoop[(it_outer + 1) % outerLoop.Length].position);

            _maskVerts[3 * it_outer] = curr;
            Vector3 mid = (curr + next) / 2.0f;
            _maskVerts[3 * it_outer + 1] = mid;
            _maskVerts[3 * it_outer + 2] = mid;

            Rigidbody2D _rbd = outerLoop[it_outer].gameObject.GetComponent<Rigidbody2D>();
            Vector3 forceVec = curr - center.position;

            //Force to inflate the blob;
            _rbd.AddForce(forceVec * inflationIntensity);



            _meshVerts[it_outer] = curr;

        }
        _meshVerts[_meshVerts.Length - 1] = center.position;
        _maskMf.mesh.vertices = _maskVerts;
        _meshMf.mesh.vertices = _meshVerts;
    }

    #endregion

    #region UTIL_METHODS
    //Circular indexing utility function
    int myMod(int p1,int p2){
        return (p1%p2 + p2)%p2;
    }
    

    //Utility vertex offsetting function
    Vector3 _offset(Vector3 pt){
        return pt + (pt-center.position)*offsetFactor;
    }


    void _initRequiredCmponents(GameObject go, out MeshFilter mf, out MeshRenderer mr)
    {
        mf = go.GetComponent<MeshFilter>();
        //If not added in editor already
        if (mf == null)
        {
            mf = go.AddComponent<MeshFilter>() as MeshFilter;
        }
        mr = go.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = go.AddComponent<MeshRenderer>() as MeshRenderer;
        }
    }
    #endregion


    // Update is called once per frame
	void FixedUpdate () {
        _updateMeshes();


	}
}
