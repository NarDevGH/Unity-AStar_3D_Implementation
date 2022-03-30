using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private NodeGrid _grid;


    public void FindPath(PathRequest request, Action<PathResult> callback) 
    {
        Vector3[] waypoints = new Vector3[0];
        bool success = false;

        Node startNode = _grid.NodeFromWorldPosition(request.pathStart);
        Node targetNode = _grid.NodeFromWorldPosition(request.pathEnd);

        if (startNode.walkable && targetNode.walkable)
        {

            Heap<Node> openNodes = new Heap<Node>(_grid.nodesAmmount);
            HashSet<Node> closeNodes = new HashSet<Node>();


            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                Node currentNode = openNodes.RemoveFirst();
                closeNodes.Add(currentNode);

                if (currentNode == targetNode)
                {
                    success = true;
                    break;
                }

                foreach (Node neighbour in _grid.GetNeighbours(currentNode))
                {
                    if (neighbour.walkable == false || closeNodes.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighboar = currentNode.distFromStart + GetDistanceBetweenNodes(currentNode, neighbour) + neighbour.movementPenalty;
                    if (openNodes.Contains(neighbour) == false || newMovementCostToNeighboar < neighbour.distFromStart)
                    {
                        neighbour.distFromStart = newMovementCostToNeighboar;
                        neighbour.distFromTarget = GetDistanceBetweenNodes(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (openNodes.Contains(neighbour) == false)
                        {
                            openNodes.Add(neighbour);
                        }
                        else 
                        {
                            openNodes.UpdateItem(neighbour);
                        }
                    }
                }

            }
        }


        if (success) 
        {
            waypoints = RetracePath(startNode, targetNode);
            success = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, success, request.callback));
    }

    private Vector3[] RetracePath(Node startNode, Node endNode) 
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) 
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Add(currentNode); // Add startNode

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    private Vector3[] SimplifyPath(List<Node> path) 
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        if (path.Count != 1)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector2 directionNew = new Vector2(path[i].gridPosition.x - path[i + 1].gridPosition.x, path[i].gridPosition.y - path[i + 1].gridPosition.y);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;
            }
        }
        else
        {
            waypoints.Add(path[0].worldPosition);
        }

        return waypoints.ToArray();
    }

    private int GetDistanceBetweenNodes(Node nodeA,Node nodeB) 
    {
        int distX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int distY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (distX > distY)
        {
            return 2*distY + 1*(distX - distY); // it is multiplied so that moving diagonally costs more, and by doing this, the pathfinding is more consistent
        }
        else 
        {
            return 2*distX + 1*(distY - distX);
        }
    }
}
