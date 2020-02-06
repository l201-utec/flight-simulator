using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

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
}


public static class Vector2Extensions
{
    public static Vector3 UnFlatten(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
}

// change name of grid
public class Graph : MonoBehaviour
{

    public int nodesPerChunkEdge;
    public GameObject mapGenerator;

    TerrainGenerator terrainGenerator;
    MeshSettings meshSettings;

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
    }

    void Update()
    {
       //Debug.Log(terrainGenerator.viewer.position.ToString("F4"));
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
        // Add other collisions here
        if (worldPosition.y <= getHeightTerrain(worldPosition))
        {
            return false;
        }
        return true;
    }

    Vector3 GetClosestNodePosition(Vector3 worldPosition)
    {
        return (worldPosition / nodeSeparation).Round() * nodeSeparation;
    }

    Node ClosestNode(Vector3 point)
    {
        Vector3 worldPosition = GetClosestNodePosition(point);
        bool walkable = CheckIfWalkable(worldPosition);

        return new Node(worldPosition, walkable);
    }

    Vector3 GetChunkIndex(Vector3 worldPosition)
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
            Debug.LogError("ChunkIndex out of bounds");
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
        if (terrainGenerator != null)
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
