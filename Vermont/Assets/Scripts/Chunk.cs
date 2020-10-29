// The Mesh part of this code is from a tutorial by Brackeys
// https://www.youtube.com/watch?v=64NblGkAabk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    // Basic chunk properties
    private int chunkID;
    private Point2D chunkCoords; // top left
    private Vector3 center;
    private int sideLength;

    // Terrain things
    private int xSize; // number of terrain squares per side (vertices per side - 1)
    private int zSize;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    private float[,] heightMap;

    public static float minTerrainHeight = 0;
    public static float maxTerrainHeight = 80;

    public static int numTrees = 150;
    public GameObject GreenTree1;
    public GameObject RedTree1;
    public GameObject OrangeTree1;
    public GameObject YellowTree1;
    public GameObject GreenTree2;
    public GameObject RedTree2;
    public GameObject OrangeTree2;
    public GameObject YellowTree2;
    public GameObject GreenTree3;
    public GameObject RedTree3;
    public GameObject OrangeTree3;
    public GameObject YellowTree3;
    public GameObject PineTree;
    public GameObject DeadTree;
    public List<GameObject> trees;


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
        GetComponent<MeshFilter>().GetComponent<MeshRenderer>().enabled = true;
        for(int i = 0; i < trees.Count; i++)
        {
            trees[i].GetComponent<MeshRenderer>().enabled = true;
        }
    }
    public void DisableDrawing()
    {
        GetComponent<MeshFilter>().GetComponent<MeshRenderer>().enabled = false;
        for(int i = 0; i < trees.Count; i++)
        {
            trees[i].GetComponent<MeshRenderer>().enabled = false;
        }
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
    public void InitializeTrees()
    {
        for(int i = 0; i < numTrees; i++)
        {
            Vector3 randomPoint = this.getRandomPoint(0);
            randomPoint.y = this.GetTerrainHeightAt(randomPoint);

            GameObject newTree;
            float treeType = Random.Range(0, 100);
            int[] cutoffs = this.GetTreeTypeCutoffs(randomPoint.y);
            if(treeType < cutoffs[0])
            {
                newTree = Instantiate(GreenTree1);
            }
            else if(treeType < cutoffs[1])
            {
                newTree = Instantiate(RedTree1);
            }
            else if(treeType < cutoffs[2])
            {
                newTree = Instantiate(OrangeTree1);
            }
            else if(treeType < cutoffs[3])
            {
                newTree = Instantiate(YellowTree1);
            }
            else if (treeType < cutoffs[4])
            {
                newTree = Instantiate(GreenTree2);
            }
            else if (treeType < cutoffs[5])
            {
                newTree = Instantiate(RedTree2);
            }
            else if (treeType < cutoffs[6])
            {
                newTree = Instantiate(OrangeTree2);
            }
            else if (treeType < cutoffs[7])
            {
                newTree = Instantiate(YellowTree2);
            }
            else if (treeType < cutoffs[8])
            {
                newTree = Instantiate(GreenTree3);
            }
            else if (treeType < cutoffs[9])
            {
                newTree = Instantiate(RedTree3);
            }
            else if (treeType < cutoffs[10])
            {
                newTree = Instantiate(OrangeTree3);
            }
            else if (treeType < cutoffs[11])
            {
                newTree = Instantiate(YellowTree3);
            }
            else if (treeType < cutoffs[12])
            {
                newTree = Instantiate(PineTree);
            }
            else
            {
                newTree = Instantiate(DeadTree);
            }
            newTree.transform.position = randomPoint;
            trees.Add(newTree);
        }
    }

    // Returns a list of the tree cutoffs based on the height of where the tree is being generated
    // Green1, Red1, Orange1, Yellow1, Green2, Red2, Orange2, Yellow2, Green3, Red3, Orange3, Yellow3, Pine
    private int[] GetTreeTypeCutoffs(float y)
    {
        if(y < -40) // No Greens
        {
            return new int[] { 0, 12, 24, 36, 36, 48, 60, 72, 72, 84, 96, 100, 100 };
        }
        else if (y < -10) // Everything
        {
            return new int[] { 3, 6, 9, 12, 22, 32, 42, 52, 62, 72, 82, 92, 99 };
        }
        else if(y < 0) // Reds
        {
            return new int[] { 1, 8, 9, 10, 14, 34, 42, 46, 48, 68, 74, 78, 96 };
        }
        else if (y < 20) // More pines
        {
            return new int[] { 2, 4, 6, 8, 13, 18, 23, 28, 33, 38, 43, 49, 90 };
        }
        else // Pines
        {
            return new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 88 };
        }
    }

    // Returns a random point on the plane of this chunk, that is not within buffer of the border
    private Vector3 getRandomPoint(float buffer)
    {
        float randomX = Random.Range(-sideLength / 2 + buffer, sideLength / 2 - buffer);
        float randomZ = Random.Range(-sideLength / 2 + buffer, sideLength / 2 - buffer);
        return center + new Vector3(randomX, 0f, randomZ);
    }

    // Return the actual 3d world coordinates of the mesh point
    public Vector3 GetVertexCoordinates(int i, int j)
    {
        float x = center.x - sideLength/2 + i*(sideLength / xSize);
        float y = minTerrainHeight + heightMap[i, j] * (maxTerrainHeight - minTerrainHeight);
        float z = center.z - sideLength / 2 + j * (sideLength / zSize);
        return new Vector3(x, y, z);
    }

    // Returns the terrain height at a given point in this chunk
    public float GetTerrainHeightAt(Vector3 p)
    {
        float localX = p.x - (center.x - sideLength / (2f));
        float localZ = p.z - (center.z - sideLength / (2f));
        int i = Mathf.FloorToInt(localX / sideLength * xSize);
        int j = Mathf.FloorToInt(localZ / sideLength * zSize);
        Vector3 p1 = this.GetVertexCoordinates(i, j);
        Vector3 p2 = this.GetVertexCoordinates(i+1, j);
        Vector3 p3 = this.GetVertexCoordinates(i, j+1);
        Vector3 p4 = this.GetVertexCoordinates(i+1, j+1);
        return Mathf.Min(p1.y, p2.y, p3.y, p4.y);
        /*
        // Check which triangle the point is in
        Vector3 v1, v2, corner;
        if(ChunkManager.distanceFormula(p.x, p.z, p2.x, p2.z) < ChunkManager.distanceFormula(p.x, p.z, p3.x, p3.z)) // up right triangle
        {
            v1 = p1 - p2;
            v2 = p4 - p2;
            corner = p2;
        }
        else // down left triangle
        {
            v1 = p4 - p3;
            v2 = p1 - p3;
            corner = p3;
        }
        Vector3 normal = Vector3.Cross(v1, v2);
        float b = normal.y;
        if(b != 0)
        {
            return (Vector3.Dot(normal, corner) - normal.x*p.x - normal.z*p.z) / b;
        }
        return corner.y;*/
    }

    public void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        int i = 0;
        float xCoord, yCoord, zCoord;
        for (int z = 0; z < zSize + 1; z++)
        {
            for (int x = 0; x < xSize + 1; x++)
            {
                xCoord = center.x - sideLength / 2 + x * sideLength / xSize;
                zCoord = center.z - sideLength / 2 + z * sideLength / zSize;
                yCoord = minTerrainHeight + heightMap[x, z] * (maxTerrainHeight - minTerrainHeight);
                vertices[i] = new Vector3(xCoord, yCoord, zCoord);
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

        uvs = new Vector2[vertices.Length];
        i = 0;
        for (int z = 0; z < zSize + 1; z++)
        {
            for (int x = 0; x < xSize + 1; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }
    }
    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }
}
