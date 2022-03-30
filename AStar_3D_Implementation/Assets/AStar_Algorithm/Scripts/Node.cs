using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    public int movementPenalty;

    public int distFromStart;
    public int distFromTarget;
    public int sumDist { get => distFromStart + distFromTarget; }

    private int _heapIndex;
    public int heapIndex { get => _heapIndex; set => _heapIndex = value; }

    public Node parent;

    public Node(Vector2Int gridPos, Vector3 worldPosition, bool walkable, int movementPenalty)
    {
        this.gridPosition = gridPos;
        this.worldPosition = worldPosition;
        this.walkable = walkable;
        this.movementPenalty = movementPenalty;
    }

    public int CompareTo(Node noteToCompare)
    {
        int compare = sumDist.CompareTo(noteToCompare.sumDist);
        if (compare == 0) 
        {
            compare = distFromTarget.CompareTo(noteToCompare.distFromTarget);
        }
        return -compare;    // -compare  bc heap value priority order
    }
}
