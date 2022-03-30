using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 20;
    [SerializeField] private float _turnDist = 5;
    [SerializeField] private float _turnSpeed = 3;

    private Path _path;

    private void Update()
    {
        Paths_Manager.RequestPath( new PathRequest(transform.position, _target.position, OnPathFound) );
    }

    private void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _path = new Path(waypoints, transform.position,_turnDist);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath() 
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(_path.lookPoints[0]);

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while ( _path.turnBoundaries[pathIndex].HasCrossedLine(pos2D) ) 
            {
                if (pathIndex == _path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else 
                {
                    pathIndex++;
                }
            }

            if (followingPath) 
            {
                Quaternion targetRotation = Quaternion.LookRotation(_path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);

                transform.Translate(Vector3.forward * Time.deltaTime * _speed,Space.Self);
            }

            yield return null;
        }
    }
}
