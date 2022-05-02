using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    //Vector2[] uvs;
    Color[] colors;

    public int xSize = 50;
    public int zSize = 50;

    public float scale = 4.5f;
    public float depth = 5f;

    public Gradient gradient;

    private float minMeshHeight;
    private float maxMeshHeight;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        CreateShape();
        UpdateMesh();
    }

    //IEnumerator CreateShape()
    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = GetNoiseSample(x, z) * depth;
                vertices[i] = new Vector3(x, y, z);

                if (y > maxMeshHeight) maxMeshHeight = y;
                if (y < minMeshHeight) minMeshHeight = y;

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
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

        //For UV array
        //uvs = new Vector2[vertices.Length];

        //for (int i = 0, z = 0; z <= zSize; z++)
        //{
        //    for (int x = 0; x <= xSize; x++)
        //    {
        //        uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
        //        i++;
        //    }
        //}

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minMeshHeight, maxMeshHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uvs;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    float GetNoiseSample(int x, int z)
    {
        float xCoord = (float)x / xSize * scale;
        float zCoord = (float)z / zSize * scale;

        float y = Mathf.PerlinNoise(xCoord, zCoord);
        return y;
    }
}
