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
}
