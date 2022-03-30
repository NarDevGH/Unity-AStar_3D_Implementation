using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public Vector2 _worldSize;
    public float _nodeRadius;
    public bool _gizmo;
    public TerrainType notWalkableRegion;
    public TerrainType[] walkableRegions;

    public int nodesAmmount { get => _size.x * _size.y; }

    private Vector2Int _size;
    private float _nodeDiameter;
    private Node[,] _grid;
    private LayerMask _walkableMask;
    private Dictionary<int, int> _walkableRegionsDict;

    private void Awake()
    {
        GetWalkableRegionsDict();

        _nodeDiameter = _nodeRadius * 2;

        GetGridSize();

        GetWalkableMask();

        CreateGrid();
    }

    

    public List<Node> GetNeighbours(Node node) 
    {
        List<Node> neighbours = new List<Node>();
        // x=-1 && y=-1 == start from bottom left
        for (int x = -1; x <= 1; x++) 
        {
            for (int y = -1; y <= 1; y++)
            {
                // if evaluated node, continue
                if (x == 0 && y == 0) 
                    continue;

                //if outside grid, continue
                if ((node.gridPosition.x + x) < 0 || (node.gridPosition.x + x) >= _size.x) continue;
                if ((node.gridPosition.y + y) < 0 || (node.gridPosition.y + y) >= _size.y) continue;

                neighbours.Add( _grid[node.gridPosition.x+x, node.gridPosition.y+y] );
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition) 
    {
        float percentX = worldPosition.x/_worldSize.x + .5f;
        float percentY = worldPosition.z/_worldSize.y + .5f;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt(Mathf.Min(_size.x * percentX, _size.x - 1));
        int y = Mathf.FloorToInt(Mathf.Min(_size.y* percentY, _size.y - 1));

        return _grid[x, y];
    }

    private void CreateGrid()
    {
        _grid = new Node[_size.x, _size.y];

        Vector3 worldBottomLeft = transform.position - Vector3.right * _worldSize.x / 2 - Vector3.forward * _worldSize.y / 2;

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                Vector3 xPos = Vector3.right * (x * _nodeDiameter + _nodeRadius);
                Vector3 yPos = Vector3.forward * (y * _nodeDiameter + _nodeRadius);
                Vector3 nodeWordlPosition = worldBottomLeft + xPos + yPos;

                bool walkable = !Physics.CheckSphere(nodeWordlPosition, _nodeRadius, notWalkableRegion.layer);
                int movemenPenalty = 0;

                // if (walkable) { get _walkableRegionsDict ...} around obstacles will have a penalty of 0 , so the path will prefer travers close to the obtacles 
                
                Ray ray = new Ray(nodeWordlPosition + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, _walkableMask)) 
                {
                    _walkableRegionsDict.TryGetValue(hit.collider.gameObject.layer, out movemenPenalty);
                }

                // around obstacles will have a higher penalty , so the path will avoid travers close to the obtacles
                if (!walkable) 
                {
                    movemenPenalty += notWalkableRegion.transitPenalty;
                }


                _grid[x, y] = new Node(new Vector2Int(x, y), nodeWordlPosition, walkable, movemenPenalty);
            }
        }

    }

    private void GetWalkableRegionsDict()
    {
        _walkableRegionsDict = new Dictionary<int, int>();
        foreach (TerrainType region in walkableRegions)
        {
            _walkableRegionsDict.Add((int)Mathf.Log(region.layer.value, 2), region.transitPenalty);
        }
    }

    private void GetGridSize()
    {
        _size.x = Mathf.RoundToInt(_worldSize.x / _nodeDiameter);
        _size.y = Mathf.RoundToInt(_worldSize.y / _nodeDiameter);
    }

    private void GetWalkableMask()
    {
        foreach (TerrainType region in walkableRegions)
        {
            _walkableMask.value += region.layer.value;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_gizmo) return;

        Gizmos.DrawWireCube(transform.position, new Vector3(_worldSize.x, 1, _worldSize.y));

        if (_grid != null) 
        {
            foreach (Node node in _grid) 
            {
                Gizmos.color = (node.walkable) ?  Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (_nodeDiameter -.1f) );
            }
        }
    }
}

[Serializable]
public struct TerrainType 
{
    public LayerMask layer;

    [Min(0)]
    public int transitPenalty;
}