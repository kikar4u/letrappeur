using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedPositionInfo
{
    public WaypointCurve nextWaypoint;
    public WaypointCurve lastWaypoint;

    public float segmentBetweenWaypoint;

    public Vector3 CalculateCurvePoint(float segment)
    {
        Vector3 nextMoveDir = Mathf.Pow(1 - segment, 3)
               * lastWaypoint.waypointPosition.transform.position +
                3 * Mathf.Pow(1 - segment, 2) * segment
                * lastWaypoint.bezierSecondPointPosition.transform.position +
                3 * (1 - segment) * Mathf.Pow(segment, 2)
                * nextWaypoint.bezierFirstPointPosition.position
                + Mathf.Pow(segment, 3) * nextWaypoint.waypointPosition.transform.position;

        return new Vector3(nextMoveDir.x, nextMoveDir.y, nextMoveDir.z);
    }

    public void SetValues(CurvedPositionInfo newCurvedPositionInfo)
    {
        nextWaypoint = newCurvedPositionInfo.nextWaypoint;
        lastWaypoint = newCurvedPositionInfo.lastWaypoint;
        segmentBetweenWaypoint = newCurvedPositionInfo.segmentBetweenWaypoint;
    }

    public float GetCurvedLength(float ratio)
    {
        Vector3 previousPos = lastWaypoint.waypointPosition.transform.position;
        float length = 0f;
        float actualStep = 0f;
        while (actualStep <= 1f)
        {
            length += Vector3.Distance(previousPos, CalculateCurvePoint(actualStep));
            previousPos = CalculateCurvePoint(actualStep);
            actualStep += ratio;
        }
        return length;
    }
}
