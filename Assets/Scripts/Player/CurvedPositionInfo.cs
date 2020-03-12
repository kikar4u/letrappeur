using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedPositionInfo
{
    //Pas de découpe de la courbe pour calculer sa longueur au plus proche
    const float RATIO = 0.002f;

    public WaypointCurve nextWaypoint;
    public WaypointCurve lastWaypoint;

    public float segmentBetweenWaypoint;

    private float curvedLength;

    public CurvedPositionInfo(WaypointCurve _lastWP, WaypointCurve _nextWP)
    {
        lastWaypoint = _lastWP;
        nextWaypoint = _nextWP;
        segmentBetweenWaypoint = 0;
        curvedLength = CalculateCurvedLength();
    }

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

    public float CalculateCurvedLength()
    {
        //Calcule la longueur de la courbe avec les informations de l'objet
        Vector3 previousPos = lastWaypoint.waypointPosition.transform.position;
        float length = 0f;
        float actualStep = 0f;
        while (actualStep <= 1f)
        {
            Vector3 actualStepCurvePoint = CalculateCurvePoint(actualStep);
            length += Vector3.Distance(new Vector3(previousPos.x, 0f, previousPos.z), new Vector3(actualStepCurvePoint.x, 0f, actualStepCurvePoint.z));
            previousPos = CalculateCurvePoint(actualStep);
            actualStep += RATIO;
        }
        return length;
    }

    public float GetCurvedLength()
    {
        return curvedLength;
    }

    public void SetCurvedLength()
    {
        curvedLength = CalculateCurvedLength();
    }
}
