using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://toqoz.fyi/thousands-of-meshes.html
public class InstancedDemo : MonoBehaviour
{
    [Range(0,1023)] //limited to 1023 for DrawMeshInstanced
    public int population;
    [Range(0,20)]
    public float range;
    public Material material;

    private Matrix4x4[] matrices;
    private MaterialPropertyBlock block;
    
    private Mesh mesh;

    void Setup(){
        Mesh mesh = CreateQuad();
        this.mesh = mesh;

        matrices = new Matrix4x4[population];
        Vector4[] colors = new Vector4[population];

        block = new MaterialPropertyBlock();

        for (int i = 0; i < population; i++){
            Matrix4x4 mat = Matrix4x4.identity;
            Vector3 postiion = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range,range));
            Quaternion rotation = Quaternion.Euler ( Random.Range(-180,180), Random.Range(-180,180), Random.Range(-180,180));
            Vector3 scale = Vector3.one;

            mat = Matrix4x4.TRS(postiion,rotation,scale);

            matrices[i] = mat;

            colors[i] = Color.Lerp(Color.red, Color.blue, Random.value);

        }
        block.SetVectorArray("_Color", colors);
    }

    //from unity docs
    private Mesh CreateQuad(float width = 1f, float height = 1f){
                // Create a quad mesh.
        var mesh = new Mesh();

        float w = width * .5f;
        float h = height * .5f;
        var vertices = new Vector3[4] {
            new Vector3(-w, -h, 0),
            new Vector3(w, -h, 0),
            new Vector3(-w, h, 0),
            new Vector3(w, h, 0)
        };

        var tris = new int[6] {
            // lower left tri.
            0, 2, 1,
            // lower right tri
            2, 3, 1
        };

        var normals = new Vector3[4] {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };

        var uv = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }
    // Update is called once per frame
    void Start(){
        Setup();
    }
    
    void Update()
    {
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, population, block);
    }
}
