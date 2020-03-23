using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedPositionInfo
{
    //Pas de découpe de la courbe pour calculer sa longueur au plus proche
    const float RATIO = 0.0001f;
    public int id;
    public WaypointCurve nextWaypoint;
    public WaypointCurve lastWaypoint;

    public float segmentBetweenWaypoint;

    private float curvedLength;

    public CurvedPositionInfo(WaypointCurve _lastWP, WaypointCurve _nextWP, int _id)
    {
        lastWaypoint = _lastWP;
        nextWaypoint = _nextWP;
        id = _id;
        segmentBetweenWaypoint = 0;
        curvedLength = CalculateCurvedLength();
    }

    //@segment compris entre 0 et 1
    public Vector3 CalculateCurvePoint(float segment)
    {
        //Calcule la position du point sur la courbe de bézier 
        Vector3 nextMoveDir = Mathf.Pow(1 - segment, 3)
               * lastWaypoint.waypointPosition.transform.position +
                3 * Mathf.Pow(1 - segment, 2) * segment
                * lastWaypoint.bezierSecondPointPosition.transform.position +
                3 * (1 - segment) * Mathf.Pow(segment, 2)
                * nextWaypoint.bezierFirstPointPosition.position
                + Mathf.Pow(segment, 3) * nextWaypoint.waypointPosition.transform.position;

        return nextMoveDir;
    }

    public void SetValues(CurvedPositionInfo newCurvedPositionInfo)
    {
        nextWaypoint = newCurvedPositionInfo.nextWaypoint;
        lastWaypoint = newCurvedPositionInfo.lastWaypoint;
    }

    public void SetSegmenentPoint(float _segment)
    {
        segmentBetweenWaypoint = _segment;
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
