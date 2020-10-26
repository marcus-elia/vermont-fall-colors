using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperChunk
{
    private int size;  // How many chunks are on each side of this
    private int superChunkID;
    private Point2D superChunkCoords;
    private int verticesPerChunk; // How many terrain vertices are on each side of a chunk
    private int heightMapSize;

    private float[,] heightMap;

    public SuperChunk(float[,] inputHeightMap, int inputSize, int inputSuperChunkID, int inputVerticesPerChunk)
    {
        heightMap = inputHeightMap;
        size = inputSize;
        superChunkID = inputSuperChunkID;
        verticesPerChunk = inputVerticesPerChunk;
        heightMapSize = size * verticesPerChunk;
        superChunkCoords = ChunkManager.chunkIDtoPoint2D(superChunkID);
    }

    public Point2D LocalCoords(Point2D chunkCoords)
    {
        int x = chunkCoords.x - superChunkCoords.x * size;
        int z = chunkCoords.z - superChunkCoords.z * size;
        if(x < 0 || x >= size || z < 0 || z >= size)
        {
            Debug.Log("The chunk is not in this superchunk.");
        }
        return new Point2D(x, z);
    }
    // Wrapper
    public Point2D LocalCoords(int chunkID)
    {
        return LocalCoords(ChunkManager.chunkIDtoPoint2D(chunkID));
    }

    public float[,] GetSubHeightMap(Point2D localCoords)
    {
        int baseX = localCoords.x * (verticesPerChunk-1);
        int baseZ = localCoords.z * (verticesPerChunk-1);
        float[,] subHeightMap = new float[verticesPerChunk, verticesPerChunk];
        for(int x = 0; x < verticesPerChunk; x++)
        {
            for(int z = 0; z < verticesPerChunk; z++)
            {
                subHeightMap[x, z] = heightMap[baseX + x, baseZ + z];
            }
        }
        return subHeightMap;
    }
}
