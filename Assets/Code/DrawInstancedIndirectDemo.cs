using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInstancedIndirectDemo : MonoBehaviour
{
    [Range(64, 1000000)]
    public int population;
    [Range(1f, 20f)]
    public float range;
    [Range(1f, 10f)]
    public float pushRange;

    public CharacterSO character;

    public Material material;
    public ComputeShader pushShader;
    public Transform pusher;

    private ComputeBuffer meshPropertiesBuffer;
    private ComputeBuffer argsBuffer;

    private Mesh mesh;
    private Bounds bounds;

    private int kernel;
    //mesh properties struct
    //not actually sure if I need this in cput
    private struct MeshProperties {
        public Matrix4x4 mat;
        public Vector4 color;

        public static int Size(){
            return 
            sizeof(float) * 4 * 4 +//matrix
            sizeof(float) * 4;
        }
    }

    void Setup(){
        Mesh mesh = CreateQuad();
        this.mesh = mesh;

        bounds = new Bounds(transform.position, Vector3.one * (range + 1 ));

        InitializeBuffers();
    }

    void InitializeBuffers(){
        kernel = pushShader.FindKernel("PushKernel");

        //argument buffer
        uint[] args = new uint[5] {0,0,0,0,0};

        // 0 is the number of triangle indices
        // 1 is population
        // others are for drawing submeshes but we send them regardless
        args[0] = (uint) mesh.GetIndexCount(0);
        args[1] = (uint) population;
        args[2] = (uint) mesh.GetIndexStart(0);
        args[3] = (uint) mesh.GetBaseVertex(0);
        argsBuffer = new ComputeBuffer( 1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
        
        //Initialize buffer with the given population
        MeshProperties[] properties = new MeshProperties[population];
        for (int i = 0; i < population; i++){
            MeshProperties props = new MeshProperties();
            Vector3 position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range,range));
            Quaternion rotation = Quaternion.Euler ( Random.Range(-180,180), Random.Range(-180,180), Random.Range(-180,180));
            Vector3 scale = Vector3.one;

            props.mat = Matrix4x4.TRS(position,rotation,scale);
            props.color = Color.Lerp(Color.red, Color.blue, Random.value);

            properties[i] = props;

        }

        meshPropertiesBuffer = new ComputeBuffer(population, MeshProperties.Size());
        meshPropertiesBuffer.SetData(properties);
        pushShader.SetBuffer(kernel, "_Properties", meshPropertiesBuffer);
        material.SetBuffer("_Properties", meshPropertiesBuffer);
    }

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
    // Start is called before the first frame update
    void Start()
    {
        population = character.Population;
        range = character.Range;
        pushRange = character.pushRange;
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        GPUPushKernel();
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
    }
    
    private void GPUPushKernel(){
        pushShader.SetFloat("_pushRange", pushRange);
        pushShader.SetVector("_PusherPosition",pusher.position);
        pushShader.Dispatch(kernel, Mathf.CeilToInt(population/64f), 1,1);
    }
    void OnDisable(){
        if(meshPropertiesBuffer != null){
            meshPropertiesBuffer.Release();
        }
        meshPropertiesBuffer = null;

        if (argsBuffer != null){
            argsBuffer.Release();
        }
        argsBuffer = null;
    }
}
