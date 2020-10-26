using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    // Basic chunk properties
    private int chunkID;
    private Point2D chunkCoords; // top left
    private Vector3 center;
    private int sideLength;
    public GameObject ground;

    // Terrain things
    private int xSize; // number of terrain squares per side (vertices per side - 1)
    private int zSize;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    private float[,] heightMap;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnableDrawing()
    {
        ground.GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshFilter>().GetComponent<MeshRenderer>().enabled = true;
    }
    public void DisableDrawing()
    {
        ground.GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshFilter>().GetComponent<MeshRenderer>().enabled = false;
    }

    // Setters
    public void setChunkID(int inputID)
    {
        chunkID = inputID;
        chunkCoords = ChunkManager.chunkIDtoPoint2D(chunkID);
    }
    public void setSideLength(int inputSideLength)
    {
        sideLength = inputSideLength;
        center = new Vector3(sideLength * chunkCoords.x + sideLength / 2.0f, 0f, sideLength * chunkCoords.z + sideLength / 2.0f);
    }
    public void setHeightMap(float[,] inputHeightMap)
    {
        heightMap = inputHeightMap;
    }
    public void setXSize(int input)
    {
        xSize = input;
    }
    public void setZSize(int input)
    {
        zSize = input;
    }
    public void InitializeMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    public void InitializeGround()
    {
        ground = Instantiate(ground);
        ground.transform.position = center;
    }
    // Returns a random point on the plane of this chunk, that is not within buffer of the border
    private Vector3 getRandomPoint(float buffer)
    {
        float randomX = Random.Range(-sideLength / 2 + buffer, sideLength / 2 - buffer);
        float randomZ = Random.Range(-sideLength / 2 + buffer, sideLength / 2 - buffer);
        return center + new Vector3(randomX, 0f, randomZ);
    }

    public void CreateShape()
    {
        vertices = new Vector3[(xSize+1) * (zSize+1)];

        int i = 0;
        float xCoord, zCoord;
        for (int z = 0; z < zSize+1; z++)
        {
            for (int x = 0; x < xSize+1; x++)
            {
                xCoord = center.x - sideLength / 2 + x * sideLength / xSize;
                zCoord = center.z - sideLength / 2 + z * sideLength / zSize;
                vertices[i] = new Vector3(xCoord, heightMap[x, z] * 10, zCoord);
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

    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
