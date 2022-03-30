using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class Paths_Manager : MonoBehaviour
{
    [SerializeField] private PathFinding _pathFinding;

    public static Paths_Manager instance;

    Queue<PathResult> _results;

    private void Awake()
    {
        HandelInstances();
        _results = new Queue<PathResult>();
    }

    private void Update()
    {
        if (_results.Count > 0) 
        {
            int itemsEnqueue = _results.Count;
            lock (_results) 
            {
                for (int i = 0; i < itemsEnqueue; i++)
                {
                    PathResult result = _results.Dequeue();
                    result.callback(result.path, result.succes);
                }
            }
        }
    }

    public static void RequestPath(PathRequest request) 
    {
        ThreadStart threadStart = delegate
        {
            instance._pathFinding.FindPath(request, instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }

    public void FinishedProcessingPath(PathResult result)
    {
        lock (_results) 
        {
            _results.Enqueue(result);
        }
    }

    

    private void HandelInstances()
    {
        if (instance != null) Destroy(this);

        instance = this;

    }


    
}
public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}

public struct PathResult
{
    public Vector3[] path;
    public bool succes;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool succes, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.succes = succes;
        this.callback = callback;
    }
}