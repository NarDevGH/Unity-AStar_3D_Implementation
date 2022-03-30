using UnityEngine;

public class PathFinding_Tester : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _turnDist;
    private Path _path;

    private void Update()
    {
            Paths_Manager.RequestPath(new PathRequest(transform.position, _target.position,OnPathFound) );
    }

    private void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful) 
        {
            _path = new Path(waypoints,transform.position,_turnDist);
        }
    }

    private void OnDrawGizmos()
    {
        if (_path != null) 
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _path.lookPoints.Length; i++)
            {
                Gizmos.DrawCube(_path.lookPoints[i], Vector3.one*.2f); ;
                if (i < _path.lookPoints.Length - 1) 
                {
                    Gizmos.DrawLine(_path.lookPoints[i], _path.lookPoints[i + 1]);
                }
            }
        }
    }
}
