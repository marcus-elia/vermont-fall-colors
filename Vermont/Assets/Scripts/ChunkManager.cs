using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point2D
{
    public int x;
    public int z;

    public Point2D(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

public class ChunkManager : MonoBehaviour
{
    public static int chunkSize = 20;
    public Transform playerTransform;
    private int currentPlayerChunkID;
    public int renderRadius = 5;
    private Dictionary<int, GameObject> allSeenChunks;
    private List<GameObject> currentChunks;
    private Dictionary<int, SuperChunk> superChunkByID;

    private static int superChunkSize = 5;
    private static int verticesPerChunk = 15;
    private static int noiseMapSize = superChunkSize * (verticesPerChunk-1) + 1;

    public GameObject ChunkPrefab;

    public static int seed = 2;
    private static int noiseScale = 40;
    private static int numOctaves = 5;

    // Start is called before the first frame update
    void Start()
    {
        superChunkByID = new Dictionary<int, SuperChunk>();
        currentPlayerChunkID = getChunkIDContainingPoint(playerTransform.position, chunkSize);
        allSeenChunks = new Dictionary<int, GameObject>();
        currentChunks = new List<GameObject>();
        updateChunks();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateCurrentPlayerChunkID())
        {
            updateChunks();
        }
    }

    // If the player enters a new chunk, return true and update the chunk id
    private bool updateCurrentPlayerChunkID()
    {
        int newChunkID = getChunkIDContainingPoint(playerTransform.position, chunkSize);
        if (newChunkID != currentPlayerChunkID)
        {
            currentPlayerChunkID = newChunkID;
            return true;
        }
        return false;
    }

    private void updateChunks()
    {
        // Disable previous chunks from drawing, and remove them from the arraylist
        for (int i = 0; i < currentChunks.Count; i++)
        {
            currentChunks[i].GetComponent<Chunk>().DisableDrawing();
        }
        currentChunks = new List<GameObject>();

        // Go through the new chunks and add/create them
        List<int> chunkIDs = getChunkIDsAroundID(currentPlayerChunkID, renderRadius);
        for (int i = 0; i < chunkIDs.Count; i++)
        {
            int id = (chunkIDs[i]);
            if (allSeenChunks.ContainsKey(id))
            {
                currentChunks.Add(allSeenChunks[id]);
                allSeenChunks[id].GetComponent<Chunk>().EnableDrawing();
            }
            else
            {
                // Check if we need to make a new SuperChunk for this Chunk
                int superChunkID = GetSuperChunkContainingChunk(id);
                SuperChunk sc;
                if (!superChunkByID.ContainsKey(superChunkID))
                {
                    Point2D superChunkCoords = chunkIDtoPoint2D(superChunkID);
                    Vector2 offset = new Vector2(superChunkCoords.x * noiseMapSize, -superChunkCoords.z * noiseMapSize);
                    float[,] newHeightMap = Noise.GenerateNoiseMap(noiseMapSize, noiseMapSize, seed, noiseScale, numOctaves, 0.5f, 2, offset);
                    sc = new SuperChunk(newHeightMap, superChunkSize, superChunkID, verticesPerChunk);
                    // Make the edges agree with previously existing superchunks
                    int idAbove = this.chunkIDAbove(superChunkID);
                    if(superChunkByID.ContainsKey(idAbove))
                    {
                        sc.setTopEdge(superChunkByID[idAbove].getBottomEdge());
                    }
                    int idBelow = this.chunkIDBelow(superChunkID);
                    if (superChunkByID.ContainsKey(idBelow))
                    {
                        sc.setBottomEdge(superChunkByID[idBelow].getTopEdge());
                    }
                    int idLeft = this.chunkIDLeft(superChunkID);
                    if (superChunkByID.ContainsKey(idLeft))
                    {
                        sc.setLeftEdge(superChunkByID[idLeft].getRightEdge());
                    }
                    int idRight = this.chunkIDRight(superChunkID);
                    if (superChunkByID.ContainsKey(idRight))
                    {
                        sc.setRightEdge(superChunkByID[idRight].getLeftEdge());
                    }
                    superChunkByID[superChunkID] = sc;
                }
                else
                {
                    sc = superChunkByID[superChunkID];
                }

                // Now make the new Chunk
                GameObject c = Instantiate(ChunkPrefab);
                c.GetComponent<Chunk>().setChunkID(id);
                c.GetComponent<Chunk>().setSideLength(chunkSize);
                c.GetComponent<Chunk>().setXSize(verticesPerChunk-1);
                c.GetComponent<Chunk>().setZSize(verticesPerChunk-1);
                c.GetComponent<Chunk>().InitializeMesh();
                c.GetComponent<Chunk>().setHeightMap(sc.GetSubHeightMap(sc.LocalCoords(id)));
                c.GetComponent<Chunk>().CreateShape();
                c.GetComponent<Chunk>().UpdateMesh();
                c.GetComponent<Chunk>().EnableDrawing();
                allSeenChunks.Add(id, c);
                currentChunks.Add(c);
            }
        }
    }

    // =======================================
    //
    //               Math Things
    //
    // =======================================
    public static int nearestPerfectSquare(int n)
    {
        int squareJumpAmount = 3;
        int curSquare = 1;
        int prevSquare = 0;
        while (curSquare < n)
        {
            prevSquare = curSquare;
            curSquare += squareJumpAmount;
            squareJumpAmount += 2;  // the difference between consecutive squares is odd integer
        }
        if (n - prevSquare > curSquare - n)
        {
            return curSquare;
        }
        else
        {
            return prevSquare;
        }
    }
    // Assuming n is a perfect square, return the square root of n as an int
    public static int isqrt(int n)
    {
        return (int)Mathf.Round(Mathf.Sqrt(n));
    }
    // Convert a ChunkID to the coordinates of the chunk
    public static Point2D chunkIDtoPoint2D(int n)
    {
        int s = nearestPerfectSquare(n);
        int sq = isqrt(s);
        if (s % 2 == 0)
        {
            if (n >= s)
            {
                return new Point2D(sq / 2, -sq / 2 + n - s);
            }
            else
            {
                return new Point2D(sq / 2 - s + n, -sq / 2);
            }
        }
        else
        {
            if (n >= s)
            {
                return new Point2D(-(sq + 1) / 2, (sq + 1) / 2 - 1 - n + s);
            }
            else
            {
                return new Point2D(-(sq + 1) / 2 + s - n, (sq + 1) / 2 - 1);
            }
        }
    }
    // Convert the coordinates of the chunk to the ChunkID
    public static int chunkCoordsToChunkID(int a, int b)
    {
        // Bottom Zone
        if (b > 0 && a >= -b && a < b)
        {
            return 4 * b * b + 3 * b - a;
        }
        // Left Zone
        else if (a < 0 && b < -a && b >= a)
        {
            return 4 * a * a + 3 * a - b;
        }
        // Top Zone
        else if (b < 0 && a <= -b && a > b)
        {
            return 4 * b * b + b + a;
        }
        // Right Zone
        else if (a > 0 && b <= a && b > -a)
        {
            return 4 * a * a + a + b;
        }
        // Only a=0, b=0 is not in a zone
        else
        {
            return 0;
        }
    }
    // Wrapper function
    public static int point2DtoChunkID(Point2D p)
    {
        return chunkCoordsToChunkID(p.x, p.z);
    }
    public static float distanceFormula(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
    public static float distance2d(Vector3 v1, Vector3 v2)
    {
        return distanceFormula(v1.x, v1.z, v2.x, v2.z);
    }
    // Get a diamond shape of chunk ids centered around the given point
    public static List<int> getChunkIDsAroundPoint(Point2D p, int radius)
    {
        List<int> result = new List<int>();

        // Start at the bottom of the diamond and work up from there
        for (int b = p.z + radius; b >= p.z - radius; b--)
        {
            int distanceFromZ = Mathf.Abs(b - p.z);
            for (int a = p.x - (radius - distanceFromZ); a <= p.x + (radius - distanceFromZ); a++)
            {
                result.Add(chunkCoordsToChunkID(a, b));
            }
        }
        return result;
    }
    // Wrapper
    public static List<int> getChunkIDsAroundID(int id, int radius)
    {
        return getChunkIDsAroundPoint(chunkIDtoPoint2D(id), radius);
    }
    // Get the chunkID of the chunk containing a given point
    public int getChunkIDContainingPoint(Vector3 p, int chunkSize)
    {
        int x = (int)Mathf.Floor(p.x / chunkSize);
        int z = (int)Mathf.Floor(p.z / chunkSize);
        return chunkCoordsToChunkID(x, z);
    }
    // Get the ID of the superchunk that the given chunk is contained in
    public int GetSuperChunkContainingChunk(int chunkID)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(chunkID);
        int superChunkX = Mathf.FloorToInt((float)chunkCoords.x / superChunkSize);
        int superChunkZ = Mathf.FloorToInt((float)chunkCoords.z / superChunkSize);
        return chunkCoordsToChunkID(superChunkX, superChunkZ);
    }

    public int chunkIDAbove(int id)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(id);
        return chunkCoordsToChunkID(chunkCoords.x, chunkCoords.z - 1);
    }
    public int chunkIDBelow(int id)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(id);
        return chunkCoordsToChunkID(chunkCoords.x, chunkCoords.z + 1);
    }
    public int chunkIDLeft(int id)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(id);
        return chunkCoordsToChunkID(chunkCoords.x - 1, chunkCoords.z);
    }
    public int chunkIDRight(int id)
    {
        Point2D chunkCoords = chunkIDtoPoint2D(id);
        return chunkCoordsToChunkID(chunkCoords.x + 1, chunkCoords.z);
    }
}

