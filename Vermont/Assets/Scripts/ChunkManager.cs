using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private int chunkID;
    private Point2D chunkCoords; // top left
    private Vector3 center;
    private int sideLength;
    public GameObject ground;


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
    }
    public void DisableDrawing()
    {
        ground.GetComponent<MeshRenderer>().enabled = false;
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
}

