using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public readonly Vector3[] lookPoints;
    public readonly Line[] turnBoundaries;
    public readonly int finishLineIndex;

    public Path(Vector3[] waypoints, Vector3 startPos, float turnDistance) 
    {
        lookPoints = waypoints;
        turnBoundaries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = Vec3ToVec2(startPos);
        for (int i = 0; i < lookPoints.Length; i++) 
        {
            Vector2 currentPoint = Vec3ToVec2(lookPoints[i]);
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i== finishLineIndex)? currentPoint : currentPoint - dirToCurrentPoint * turnDistance;
            turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDistance);
            previousPoint = turnBoundaryPoint;
        }
    }

    Vector2 Vec3ToVec2(Vector3 v3) 
    {
        return new Vector2(v3.x, v3.z);
    }
}
