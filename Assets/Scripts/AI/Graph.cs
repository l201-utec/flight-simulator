using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using Priority_Queue;

public static class Vector3Extensions
{
    public static Vector3 Round(this Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }
    public static Vector3 Floor(this Vector3 v)
    {
        return new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
    }
    public static Vector2 Flatten(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    public static float Manhattan(this Vector3 v, Vector3 u)
    {
        return Mathf.Abs(v.x - u.x) + Mathf.Abs(v.y - u.y) + Mathf.Abs(v.z - u.z);
    }
}


public static class Vector2Extensions
{
    public static Vector3 UnFlatten(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
}

public class Mapping<K>
{
    private Dictionary<K, float> dict = new Dictionary<K, float>();
    public float this[K key]
    {
        get {
            if (dict.ContainsKey(key))
                return dict[key];
            else
                return Mathf.Infinity;
        }
        set {
            dict[key] = value;
        }
    }
}


// change name of grid
public class Graph : MonoBehaviour
{
    public bool debug = false;
    public int maxIterations = 500;
    public int nodesPerChunkEdge;
    public GameObject mapGenerator;
    public GameObject enemySpawnManagerGObject;

    TerrainGenerator terrainGenerator;
    public MeshSettings meshSettings;
    EnemySpawnManager enemySpawnManager;
    private static Vector3[] directions = new Vector3[6]{Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

    float nodeSeparation;

    void Start()
    {
       if (mapGenerator == null) 
       {
           throw new System.Exception("Map Generator is necessary.");
       }
       // Gets script of map generator to access its data
       terrainGenerator = mapGenerator.GetComponent<TerrainGenerator>();
       meshSettings = terrainGenerator.meshSettings;

       // calculate nodeSeparation
       nodeSeparation = meshSettings.meshWorldSize / nodesPerChunkEdge;

       Debug.Log("Nodes Per Chunk Edge: " + nodesPerChunkEdge.ToString());
       Debug.Log("numVertsPerLine: " + meshSettings.numVertsPerLine);
       Debug.Log("meshWorldSize: " + meshSettings.meshWorldSize);

       enemySpawnManager = enemySpawnManagerGObject.GetComponent<EnemySpawnManager>();
    }

    List<Node> GetNeighbors(Node u)
    {
        List<Node> neighbors = new List<Node>();

        foreach (Vector3 dir in directions)
        {
            Vector3 tentative = u.worldPosition + nodeSeparation * dir;
            if (CheckIfWalkable(tentative))
                neighbors.Add(new Node(tentative));
        }

        return neighbors;
    }

    List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> path = new List<Node>(){current};
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return path;
    }
    
    public bool IsValidPosition(Vector3 worldPosition)
    {
        if (terrainGenerator.terrainChunkDictionary.ContainsKey(GetChunkIndex(worldPosition).Flatten()))
            return true;
        else
            return false;
    }

    public List<Node> AStar(Node src, Node dst)
    {
        int iterations = 0;

        System.Func<Node, Node, float> d = (u, v) => u.worldPosition.Manhattan(v.worldPosition);
        System.Func<Node, float> h = v => dst.worldPosition.Manhattan(v.worldPosition);
        Mapping<Node> g = new Mapping<Node>(); //defaults to infinity
        Mapping<Node> f = new Mapping<Node>(); //defaults to infinity

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        g[src] = 0f;
        f[src] = h(src);

        SimplePriorityQueue<Node> pq = new SimplePriorityQueue<Node>();
        pq.Enqueue(src, f[src]);

        // Debug.Log("[A* PATHFINDING]> START: " + src.worldPosition.ToString() + " TARGET: " + dst.worldPosition.ToString());

        while (iterations < maxIterations && pq.Count != 0)
        {
            Node current = pq.Dequeue();
            // Debug.Log("Current node is: " + current.worldPosition.ToString() + " with FCOST: " + f[current] + " GCOST: " + g[current] + " HCOST: " + h(current));
            
            // too precise, less precision
            if (dst.worldPosition == current.worldPosition || h(current) < 0.01)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach( Node neighbor in GetNeighbors(current))
            {
                //Debug.Log("Considering neighbor node: " + neighbor.worldPosition);
                float tentativeGScore = g[current] + d(current, neighbor);
                //Debug.Log("tentativeGscore: " + tentativeGScore + " gCOST: " + g[neighbor]);
                if (tentativeGScore < g[neighbor])
                {
                    cameFrom[neighbor] = current;
                    g[neighbor] = tentativeGScore;
                    f[neighbor] = g[neighbor] + h(neighbor);

                    if (!pq.Contains(neighbor))
                    {
                        //Debug.Log("Enqueuing node: " + neighbor.worldPosition.ToString());
                        pq.Enqueue(neighbor, f[neighbor]);
                    }
                    else
                        pq.UpdatePriority(neighbor, f[neighbor]);
                }


            }
        }
        return new List<Node>();
    }


    float getHeightTerrain(Vector3 worldPosition)
    {

        Vector3 worldChunkPosition = GetWorldPositionFromChunkIndex(
                                            GetChunkIndex(worldPosition));
        Vector3 relativePosition = worldPosition + 
                                (new Vector3(1, 0, 1) * meshSettings.meshWorldSize/2) - 
                                worldChunkPosition;

        int length = meshSettings.numVertsPerLine;

        int i  = Mathf.FloorToInt(relativePosition.x * length/meshSettings.meshWorldSize);
        int j  = length - Mathf.FloorToInt(relativePosition.z * length/meshSettings.meshWorldSize) - 1;
        
        if (i < 0 || i >= length || j < 0 || j >= length)
        {
            Debug.LogError("OutBounds: (" + i.ToString() + ", " + j.ToString() + ")");
        }
        return GetTerrainChunk(worldPosition).heightMap
            .values[i, j];
    }

    bool CheckIfWalkable(Vector3 worldPosition)
    {

        if (!IsValidPosition(worldPosition))
            return false;

        // Add other collisions here
        Collider[] hitColliders = Physics.OverlapSphere(worldPosition, nodeSeparation/2);

        foreach (Collider c in hitColliders)
        {
            // The node where the player is must be available
            if (c.tag == "EnemyAI")
                return true;
        }

        if (worldPosition.y <= getHeightTerrain(worldPosition) || hitColliders.Length != 0)
        {
            return false;
        }
        return true;
    }

    Vector3 GetClosestNodePosition(Vector3 worldPosition)
    {
        return (worldPosition/nodeSeparation).Round() * nodeSeparation;
    }

    public Node GetClosestNode(Vector3 point)
    {
        Vector3 worldPosition = GetClosestNodePosition(point);

        return new Node(worldPosition);
    }

    public Vector3 GetChunkIndex(Vector3 worldPosition)
    {
        // Since the worldPosition of indexes are the center of the chunk, we need to subtract
        // half of the diameter of that chunk
        
        Vector3 halfOffset = (meshSettings.meshWorldSize / 2) * new Vector3(1, 0, 1);

        return ((worldPosition + halfOffset) / meshSettings.meshWorldSize).Floor();
    }


    TerrainChunk GetTerrainChunk(Vector3 worldPosition)
    {
        Vector2 terrainChunkIndex = GetChunkIndex(worldPosition).Flatten();

        if (terrainGenerator.terrainChunkDictionary.ContainsKey(terrainChunkIndex))
        {
            return terrainGenerator.terrainChunkDictionary[terrainChunkIndex];
        }
        else
        {
            Debug.LogError("ChunkIndex out of bounds: " + terrainChunkIndex.ToString() + " from worldPosition: " + worldPosition.ToString());
            return null;
        }
    }

    Vector3 GetWorldPositionFromChunkIndex(Vector3 chunkIndex)
    {
        return chunkIndex * meshSettings.meshWorldSize;
    }

    void LogSummaryTerrainChunk(TerrainChunk tc)
    {
        // Summarize properties of terrain chunk
        Debug.Log("------------ SUMMARY -------------");
        Debug.Log("INDEX: " + tc.coord.ToString());
        Debug.Log("WORLD: " + GetWorldPositionFromChunkIndex(tc.coord.UnFlatten()).ToString());
        Debug.Log("----------------------------------");
    }

    void LogAllVisibleTerrainChunks(List<TerrainChunk> visibleTerrainChunks)
    {
        Debug.Log("Visible: " + 
                string.Join("", 
                    visibleTerrainChunks
                    .ConvertAll(tc => tc.coord.ToString())
                    .ToArray()));
    }

    void OnDrawGizmos()
    {
        if (debug && terrainGenerator != null)
        {
            foreach(TerrainChunk tc_ in terrainGenerator.visibleTerrainChunks)
            {
                // LogSummaryTerrainChunk(tc_);

                Vector3 center_ = GetWorldPositionFromChunkIndex(tc_.coord.UnFlatten());
                
                // Draw ChunkIndex Tags on Gizmos 
                Handles.Label(center_, tc_.coord.ToString());
                
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(center_, new Vector3(10, 500, 10));


                for (int xi = 0; xi < nodesPerChunkEdge; ++xi) 
                {
                    for (int yi = 0; yi < nodesPerChunkEdge; ++yi) 
                    {
                        for (int zi = 0; zi < nodesPerChunkEdge; ++zi) 
                        {
                            Vector3 nodePosition = center_ + nodeSeparation * 
                                                new Vector3(xi - nodesPerChunkEdge/2,
                                                            yi, 
                                                            zi - nodesPerChunkEdge/2);
                            nodePosition = nodePosition.Floor();
                            
                            Vector2 a = tc_.coord;
                            Vector2 b = GetChunkIndex(nodePosition).Flatten();
                            
                            if (a != b)
                            {
                                Debug.LogError(a.ToString() + " != " + b.ToString() + " on nodePosition: " + nodePosition.ToString());
                            }


                            bool walkable = CheckIfWalkable(nodePosition);
                            Gizmos.color = (walkable) ? Color.green: Color.red;
                            Gizmos.DrawSphere(nodePosition, 5);
                            
                            
                        }
                    }
                }
                
            }
        
        }



    }
}
