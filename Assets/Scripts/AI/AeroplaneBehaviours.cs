﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityEngine.Assertions;
using System;

public class AeroplaneBehaviours : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject targetObj;
    public WaypointProgressTracker waypointProgressTracker;
    public GameObject graphManager;
    
    private Graph graph;

    private GameObject waypointCircuitManager;
    private WaypointCircuit waypointCircuit;
    private List<Transform> sleepingTargets = new List<Transform>();

    public float distanceBetween;
    float predictionTime = 2f;


    private float nextUpdate = 1f;
    public float delay = 1f;
    public int difficulty = 0;

    void Start()
    {
        if (graphManager == null)
            graphManager = GameObject.FindGameObjectWithTag("GraphManager");
        if (targetObj == null)
            targetObj = GameObject.FindGameObjectWithTag("Player");


        waypointProgressTracker = gameObject.GetComponent<WaypointProgressTracker>();


        waypointCircuit = waypointProgressTracker.circuit;

        waypointCircuitManager = waypointCircuit.gameObject;

        graph = graphManager.GetComponent<Graph>();

        AlterWaypointCircuitFromPath(new List<Node>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (difficulty > 0 && Time.time >= nextUpdate && graph.IsValidPosition(this.transform.position))
        {
            nextUpdate = Time.time + delay;
            Debug.Log(this.gameObject.name + " is on a valid position, starting smart pathfinding");
            List<Node> path = graph.AStar(
            graph.GetClosestNode(this.transform.position),
            graph.GetClosestNode(targetObj.transform.position));
            
            if(path.Count == 0)
                Debug.Log("ASTAR: couldn't reach target");
            else
            {
                Debug.Log("PATH (s: "+ path.Count + ") of " + gameObject.name + ">> " + string.Join("", path.ConvertAll(v => v.worldPosition.ToString() ).ToArray()));
                foreach (Node v in path) 
                {
                    Debug.Log(v.worldPosition.ToString());
                }


            }
            AlterWaypointCircuitFromPath(path);
        }
        else
            AlterWaypointCircuitFromPath(new List<Node>());
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
        obj.gameObject.SetActive(false);
        sleepingTargets.Add(obj);
    }

    GameObject InstantiateTarget()
    {
        GameObject target = new GameObject("Waypoint");
        target.transform.parent = waypointCircuitManager.transform;
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
            waypoints[i].position = path[i].worldPosition;
    
        waypointCircuit.waypointList.items = waypoints.ToArray();
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