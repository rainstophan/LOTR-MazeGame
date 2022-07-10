using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ShireTerrainGenerator : MonoBehaviour
{
    Mesh mesh;
   
    private float MESH_SCALE = 0.12f;
    private float minTerrainheight;
    private float maxTerrainheight;

    public GameObject[] spawnObjects;
    public Vector3 objectScale;
    public int density;

    [SerializeField] private AnimationCurve heightCurve;
    private Vector3[] vertices;
    private int[] triangles;
    

    public int xSize;
    public int zSize;

    public float scale;
    public int octaves;
    public float lacunarity;

    public int seed;
    private float lastNoiseHeight;

    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateNewMap();
    }

    private void SetNullProperties() {
        if (xSize <= 0) xSize = 50;
        if (zSize <= 0) zSize = 50;
        if (octaves <= 0) octaves = 5;
        if (lacunarity <= 0) lacunarity = 2;
        if (scale <= 0) scale = 50;
    }

    public void CreateNewMap() {
        CreateMeshShape();
        CreateTriangles();
        UpdateMesh();
    }

    private void CreateMeshShape() {
        // Creates seed
        Vector2[] octaveOffsets = GetOffsetSeed();

        if (scale <= 0) scale = 0.0001f;

        // Create vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);

                float distance_x = Mathf.Abs(x - xSize * 0.5f);
                float distance_z = Mathf.Abs(z - zSize * 0.5f);
                //float distance = Mathf.Sqrt(distance_x*distance_x + distance_z*distance_z); // circular mask
                float distance = Mathf.Max(distance_x, distance_z); // square mask

                float max_width = xSize * 0.5f - 0.5f;
                float delta = distance / max_width;
                float gradient = delta * delta;

                noiseHeight *= Mathf.Max(0.0f, 1.0f - gradient)*0.5f;
                vertices[i] = new Vector3(x, noiseHeight, z);

                i++;
            }
        }
    }

    private Vector2[] GetOffsetSeed() {
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

    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets) {
        float amplitude = 20;
        float frequency = 1;
        float persistence = 0.5f;
        float noiseHeight = 0;

        // loop over octaves
        for (int o = 0; o < octaves; o++) {
            float mapZ = z / scale * frequency + octaveOffsets[o].y;
            float mapX = x / scale * frequency + octaveOffsets[o].x;

            //The *2-1 is to create a flat floor level
            float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }
        return noiseHeight*(0.5f);
    }

    private void SetMinMaxHeights(float noiseHeight) {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }

    private void CreateTriangles() {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        
        for (int z = 0; z < xSize; z++) {
           
            for (int x = 0; x < xSize; x++) {
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


    
    private void MapEmbellishments() {
        for (int i = 0; i < vertices.Length; i=i+density) {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            // Stop generation if height difference between 2 vertices is too steep
            if (System.Math.Abs(lastNoiseHeight - worldPt.y) < 25) {

                if (Random.Range(1, 5) == 1) {
                    GameObject objectToSpawn = spawnObjects[Random.Range(0, spawnObjects.Length)];
                    objectToSpawn.transform.localScale = objectScale;

                    Instantiate(objectToSpawn, worldPt, Quaternion.identity, transform);

                    //Debug.Log(noiseHeight);
                }
            }
            lastNoiseHeight = noiseHeight;
        }
    }

    private void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
       
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.transform.localScale = new Vector3(MESH_SCALE, MESH_SCALE * 0.4f, MESH_SCALE);

        MapEmbellishments();
    }
}