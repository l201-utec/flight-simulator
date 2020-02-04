using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

public static class Vector3Extensions
{
    public static Vector3 Round(this Vector3 aVec)
    {
        return new Vector3(Mathf.Round(aVec.x), Mathf.Round(aVec.y), Mathf.Round(aVec.z));
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

    float getHeightTerrain2(Vector3 worldPosition, Vector2 coord)
    {

        Vector3 chunkWorldPosition = GetWorldPositionFromChunkIndex(
                                            UnFlattenVector2(coord));
        Vector3 relativePosition = worldPosition - chunkWorldPosition;

        int i  = Mathf.FloorToInt(relativePosition.x * meshSettings.numVertsPerLine/meshSettings.meshWorldSize);
        int j  = Mathf.FloorToInt(relativePosition.z * meshSettings.numVertsPerLine/meshSettings.meshWorldSize);

        return GetTerrainChunk(worldPosition).heightMap.values[i, j];
    }


    float getHeightTerrain(Vector3 worldPosition)
    {

        Vector3 index = GetChunkIndex(worldPosition);
        Vector3 chunkPosition = index * meshSettings.meshWorldSize;
        Vector3 relativePosition = worldPosition - chunkPosition;

        int i  = Mathf.FloorToInt(relativePosition.x * meshSettings.numVertsPerLine/meshSettings.meshWorldSize);
        int j  = Mathf.FloorToInt(relativePosition.z * meshSettings.numVertsPerLine/meshSettings.meshWorldSize);

        return GetTerrainChunk(worldPosition).heightMap.values[i, j];
    }

    bool CheckIfWalkable(Vector3 worldPosition)
    {
        if (worldPosition.y >= getHeightTerrain(worldPosition))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Node ClosestNode(Vector3 point)
    {
        Vector3 position = new Vector3(
            Mathf.RoundToInt(point.x/nodeSeparation) * nodeSeparation,
            Mathf.RoundToInt(point.y/nodeSeparation) * nodeSeparation,
            Mathf.RoundToInt(point.z/nodeSeparation) * nodeSeparation
        );

        bool walkable = CheckIfWalkable(position);
        return new Node(position, walkable);
    }
    Vector3 GetChunkIndex(Vector3 position)
    {
        // Since the worldPosition of indexes are the center of the chunk, we need to subtract
        // half of the diameter of that chunk
        
        Vector3 halfOffset = (meshSettings.meshWorldSize / 2) * new Vector3(1, 0, 1);

        return ((position - halfOffset) / meshSettings.meshWorldSize).Round();
    }

    Vector2 FlattenVector3(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    Vector3 UnFlattenVector2(Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    TerrainChunk GetTerrainChunk(Vector3 worldPosition)
    {
        Vector2 terrainChunkIndex = FlattenVector3(GetChunkIndex(worldPosition));

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

    Vector3 GetWorldPositionFromChunkIndex(Vector2 chunkIndex)
    {
        return new Vector3(chunkIndex.x * meshSettings.meshWorldSize, 0, chunkIndex.y * meshSettings.meshWorldSize);
    }

    void LogSummaryTerrainChunk(TerrainChunk tc)
    {
        // Summarize properties of terrain chunk
        Debug.Log("------------ SUMMARY -------------");
        Debug.Log("INDEX: " + tc.coord.ToString());
        Debug.Log("WORLD: " + GetWorldPositionFromChunkIndex(tc.coord).ToString());
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
            /*
            TerrainChunk tc = GetTerrainChunk(terrainGenerator.viewer.position);

            Vector3 center = new Vector3(
                tc.coord.x * meshSettings.meshWorldSize,
                0, 
                tc.coord.y * meshSettings.meshWorldSize);

            Vector3 size = Vector3.one * meshSettings.meshWorldSize;

            Gizmos.DrawWireCube(center, size);
            
            Vector3 index = GetChunkIndex(terrainGenerator.viewer.position);
            Vector2 groundIndex = new Vector2(index.x, index.z);
            Vector3 center2 = new Vector3(
                groundIndex.x * meshSettings.meshWorldSize,
                0, 
                groundIndex.y * meshSettings.meshWorldSize);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center2, size);
            */

            // draw position
            LogAllVisibleTerrainChunks(terrainGenerator.visibleTerrainChunks);

            foreach(TerrainChunk tc_ in terrainGenerator.visibleTerrainChunks)
            {
                LogSummaryTerrainChunk(tc_);

                Vector3 center_ = GetWorldPositionFromChunkIndex(
                                            UnFlattenVector2(tc_.coord));
                
                // Draw ChunkIndex Tags on Gizmos 
                Handles.Label(center_, tc_.coord.ToString());
                
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(center_, new Vector3(10, 500, 10));


                int start = -nodesPerChunkEdge/2; 
                int end = nodesPerChunkEdge/2;

                for (int xi = start; xi < end; ++xi) 
                {
                    for (int yi = start; yi < end; ++yi) 
                    {
                        for (int zi = start; zi < end; ++zi) 
                        {
                            Vector3 nodePosition = center_ + nodeSeparation * 
                                                    new Vector3(xi, yi, zi);
                            
                            Vector2 a = tc_.coord;
                            Vector2 b = FlattenVector3(GetChunkIndex(nodePosition));
                            
                            if (a != b)
                            {
                                Debug.LogError(a.ToString() + " != " + b.ToString() + " on nodePosition: " + FlattenVector3(nodePosition).ToString());
                            }


                            // bool walkable = CheckIfWalkable(nodePosition);
                            //bool walkable = false;
                            // if (nodePosition.y > getHeightTerrain2(nodePosition, tc_.coord))
                            //     walkable = true;
                            // Gizmos.color = (walkable) ? Color.green: Color.red;
                            // if (!walkable)
                            // {
                            //     Gizmos.DrawSphere(nodePosition, 10);
                            // }
                            
                            
                        }
                    }
                }
            }
        
        }



    }
}
