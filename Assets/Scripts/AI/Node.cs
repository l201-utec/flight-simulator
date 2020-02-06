using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Node : FastPriorityQueueNode
{
    public Vector3 worldPosition;

    public Node(Vector3 _worldPosition)
    {
        worldPosition = _worldPosition;
    }
}
