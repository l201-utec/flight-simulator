using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityEngine.Assertions;
using System;
using UnityStandardAssets.Vehicles.Aeroplane;


public class AeroplaneBehaviours : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject targetObj;
    public GameObject waypointsPrefab;
    public WaypointProgressTracker waypointProgressTracker;
    public GameObject graphManager;
    public EnemySpawnManager enemySpawnManager;
    
    private Graph graph;

    private GameObject waypointCircuitManager;
    private WaypointCircuit waypointCircuit;
    private List<Transform> sleepingTargets = new List<Transform>();

    private AeroplaneAiControl aeroplaneAiControl;

    float predictionTime = 2f;
    public float facingAngle = 30.0f;


    private float nextUpdate = 0.1f;
    public float delay = 1f;
    public int difficulty = 0;

    void Start()
    {
        GameObject enemySpawner;

        if (graphManager == null)
            graphManager = GameObject.FindGameObjectWithTag("GraphManager");
        if (targetObj == null)
            targetObj = GameObject.FindGameObjectWithTag("Player");
        
        enemySpawner = GameObject.FindGameObjectWithTag("EnemySpawner");
        enemySpawnManager = enemySpawner.GetComponent<EnemySpawnManager>();

        waypointCircuitManager = Instantiate(waypointsPrefab, Vector3.zero, Quaternion.identity);
        waypointCircuitManager.name = gameObject.name + "\'s Waypoints";
        
        waypointProgressTracker = this.gameObject.GetComponent<WaypointProgressTracker>();

        waypointCircuit = waypointCircuitManager.GetComponent<WaypointCircuit>();

        waypointProgressTracker.circuit = waypointCircuit;

        graph = graphManager.GetComponent<Graph>();

        aeroplaneAiControl = this.gameObject.GetComponent<AeroplaneAiControl>();

        AlterWaypointCircuitFromPath(new List<Node>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ChangeThrottle();

        if (difficulty == 0 || !graph.IsValidPosition(this.transform.position))
        {
            AlterWaypointCircuitFromPath(new List<Node>());
        }
        else if (difficulty > 0 && Time.time >= nextUpdate && graph.IsValidPosition(this.transform.position))
        {
            nextUpdate = Time.time + delay + delay * UnityEngine.Random.Range(0f, 1f);
            Debug.Log(this.gameObject.name + " is on a valid position, starting smart pathfinding");
            List<Node> path = graph.AStar(
            graph.GetClosestNode(this.transform.position),
            graph.GetClosestNode(targetObj.transform.position));
            
            if(path.Count == 0)
                Debug.Log("ASTAR: couldn't reach target");
            else
            {
                Debug.Log("PATH (s: "+ path.Count + ") of " + gameObject.name + ">> " + string.Join("", path.ConvertAll(v => v.worldPosition.ToString() ).ToArray()));


            }

            // Remove starting position
            int frontTrash = 2;
            while (frontTrash-- > 0 && path.Count >= 2)
                path.RemoveAt(0);

            AlterWaypointCircuitFromPath(path);
        }
        else if (difficulty > 0)
            UpdateLast();
    }

    void OnDestroy()
    {
        enemySpawnManager.EnemyDefeated();
        Destroy(waypointCircuitManager);
    }

    void ChangeThrottle()
    {
        float angle = Vector3.Angle(targetObj.transform.forward, transform.position - targetObj.transform.position);
        if (angle < facingAngle)
        {
            aeroplaneAiControl.throttleInput = 0.5f + 0.5f * (facingAngle - angle)/facingAngle;
        }
        else
        {
            aeroplaneAiControl.throttleInput = 0.2f;
        }
    }
    void UpdateLast()
    {
        waypointCircuit.waypointList.items[waypointCircuit.waypointList.items.Length - 1].position = targetObj.transform.position;
    }

    Transform WakeUpTarget()
    {
        int last = sleepingTargets.Count - 1;
        Transform obj = sleepingTargets[last];
        sleepingTargets.RemoveAt(last);

        obj.gameObject.SetActive(true);
        return obj;
    }

    void PutTargetToSleep(Transform obj)
    {
        obj.gameObject.name = "Waypoint (Deactivated)";
        obj.gameObject.SetActive(false);
        sleepingTargets.Add(obj);
    }

    GameObject InstantiateTarget()
    {
        GameObject target = new GameObject("Waypoint");
        target.transform.SetParent(waypointCircuitManager.transform, false);
        // maybe one needs to add the reference to parent children
        return target;
    }

    List<Transform> RequestWaypointObjects(int quantity)
    {
        if (quantity < 0)
            return new List<Transform>();
            
        int nWaypoints = waypointCircuit.waypointList.items.Length;
        int dif = nWaypoints - quantity;

        if (dif < 0)
        {
            // I have less objects than I need
            Array.Resize(ref waypointCircuit.waypointList.items, quantity);
            
            int dif2 = dif + sleepingTargets.Count;

            while(dif2++ < 0)
            {
                // In case there are not enough sleeping targets, instantiate

                GameObject remainder = InstantiateTarget();
                PutTargetToSleep(remainder.transform);
            }

            for (int i = 0; i < Mathf.Abs(dif); ++i)
            {
                waypointCircuit.waypointList.items[nWaypoints + i] = WakeUpTarget();
            }
        }
        else if (dif > 0 )
        {
            // I have more objects than I need, deactivate
            for (int i = 0; i < dif; ++i )
            {
                PutTargetToSleep(waypointCircuit.waypointList.items[nWaypoints - i - 1]);

            }
            Array.Resize(ref waypointCircuit.waypointList.items, quantity);

        }

        return new List<Transform>(waypointCircuit.waypointList.items);

    }

    void AlterWaypointCircuitFromPath(List<Node> path)
    {

        List<Transform> waypoints;

        if (path.Count == 0)
        {
            waypoints = RequestWaypointObjects(2);
            path.Add(new Node(targetObj.transform.position));
            path.Add(new Node(targetObj.transform.position));
        }
        else if (path.Count == 1)
        {
            waypoints = RequestWaypointObjects(2);
            path.Add(path[0]);
        }
        else
            waypoints = RequestWaypointObjects(path.Count);
       
        if (waypoints.Count != path.Count) 
            Debug.LogError("Not equal number of waypoints as path length");

        for (int i = 0; i < waypoints.Count; ++i)
        {
            waypoints[i].gameObject.name = "Waypoint " + i.ToString();
            waypoints[i].position = path[i].worldPosition;
        }
    
        waypointCircuit.waypointList.items = waypoints.ToArray();
        waypointCircuit.RefreshPath();
        waypointProgressTracker.Reset();
    }


    void DumbPursue(Vector3 target)
    {
        // get better prediction
        Vector3 futurePosition = targetObj.transform.position + 
                (targetObj.transform.forward * (Time.deltaTime * predictionTime));

        // Seek(futurePosition);
    }
}
