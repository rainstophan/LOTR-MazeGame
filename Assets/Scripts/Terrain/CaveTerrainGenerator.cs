using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CaveTerrainGenerator : MonoBehaviour
{
    Mesh mesh;
   
    private float MESH_SCALE = 0.12f;

    public GameObject[] spawnObjects;
    public Vector3[] objectsScale;
    public int density;
    public float spawnCollisionCheckRadius;

    [SerializeField] private AnimationCurve heightCurve;
    private Vector3[] vertices;
    private int[] triangles;
   
    private float minTerrainheight;
    private float maxTerrainheight;

    public int xSize;
    public int zSize;

    public float scale; 
    public int octaves;
    public float lacunarity;

    public int seed;

    private float lastNoiseHeight;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateNewMap();
    }

    private void SetNullProperties() 
    {
        if (xSize <= 0) xSize = 50;
        if (zSize <= 0) zSize = 50;
        if (octaves <= 0) octaves = 5;
        if (lacunarity <= 0) lacunarity = 2;
        if (scale <= 0) scale = 50;
    } 

    public void CreateNewMap()
    {
        CreateMeshShape();
        CreateTriangles();
    
        UpdateMesh();
    }

    private void CreateMeshShape ()
    {
        // Creates seed
        Vector2[] octaveOffsets = GetOffsetSeed();

        if (scale <= 0) scale = 0.0001f;
            
        // Create vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // Set height of edge vertices to 0
                // Set height of other vertices to nosie height
                if( x <= 2 || x >= xSize-2 || z <= 2 || z >= zSize-2){
                    vertices[i] = new Vector3(x, 0, z);
                } else {
                    float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                    SetMinMaxHeights(noiseHeight);
                    vertices[i] = new Vector3(x, noiseHeight, z);
                }
                i++;
            }
        }

        
    }

    private Vector2[] GetOffsetSeed()
    {
        seed = Random.Range(0, 1000);
        
        // changes area of map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
                    
        for (int o = 0; o < octaves; o++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
    {
        float amplitude = 20;
        float frequency = 1;
        float persistence = 0.5f;
        float noiseHeight = 0;

        // loop over octaves
        for (int y = 0; y < octaves; y++)
        {
            float mapZ = z / scale * frequency + octaveOffsets[y].y;
            float mapX = x / scale * frequency + octaveOffsets[y].x;

            //The *2-1 is to create a flat floor level
            float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }
        return noiseHeight*(-1);
    }

    private void SetMinMaxHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }


    private void CreateTriangles() 
    {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        
        for (int z = 0; z < xSize; z++)
        {
           
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }


    private void MapEmbellishments() 
    {
        for (int i = 0; i < vertices.Length; i+=density )
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            // Stop generation if height difference between 2 vertices is too steep
            if(System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                
                if (Random.Range(1, 5) == 1)
                {
                    int objectIndex = Random.Range(0, spawnObjects.Length);
                    
                    GameObject objectToSpawn = spawnObjects[objectIndex];
                    objectToSpawn.transform.localScale = objectsScale[objectIndex];

                    // Check collision 
                    // Box collidor for spawned objects is requied
                    if(!Physics.CheckSphere(worldPt,spawnCollisionCheckRadius)){ 
                        Instantiate(objectToSpawn, worldPt, Quaternion.identity, transform);
                    }                
                }
                
            }
            lastNoiseHeight = noiseHeight;
            
        }

    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
     
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.transform.localScale = new Vector3(MESH_SCALE, MESH_SCALE, MESH_SCALE);

        MapEmbellishments();
    }

}
