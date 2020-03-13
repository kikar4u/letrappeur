﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCurve : MonoBehaviour
{
    //The resolution of the line
    [SerializeField] float resolution;

    public WaypointCurve[] waypointCurves;

    private CurvedPositionInfo[] curves;
    Vector3 A, B, C, D, E, F;

    //Display without having to press play
    void OnDrawGizmos()
    {

        for (int i = 1; i < waypointCurves.Length; i++)
        {
            A = waypointCurves[i].waypointPosition.transform.position;
            B = waypointCurves[i].bezierFirstPointPosition.position;
            C = waypointCurves[i].bezierSecondPointPosition.position;
            D = waypointCurves[i - 1].bezierFirstPointPosition.position;
            E = waypointCurves[i - 1].bezierSecondPointPosition.position;
            F = waypointCurves[i - 1].waypointPosition.transform.position;

            //The Bezier curve's color
            Gizmos.color = Color.green;

            //The start position of the line
            Vector3 lastPos = A;

            //How many loops
            int loops = Mathf.FloorToInt(1f / resolution);

            for (int j = 1; j <= loops; j++)
            {
                //Which t position are we at?
                float t = j * resolution;

                //Find the coordinates between the control points with a Catmull-Rom spline
                Vector3 newPos = DeCasteljausAlgorithm(t);
                //Draw this line segment
                Gizmos.DrawLine(lastPos, newPos);

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
            }

            //Also draw lines between the control points and endpoints
            Gizmos.color = Color.green;

            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(A, C);
            Gizmos.DrawLine(D, F);
            Gizmos.DrawLine(E, F);
        }
    }

    private void Awake()
    {
        curves = new CurvedPositionInfo[waypointCurves.Length - 1];
        for (int i = 0; i < curves.Length; i++)
        {
            curves[i] = new CurvedPositionInfo(waypointCurves[i], waypointCurves[i + 1], i);
        }
    }

    public CurvedPositionInfo GetCurvePosInfoAtIndex(int id)
    {
        return curves[id];
    }

    Vector3 DeCasteljausAlgorithm(float t)
    {
        //Linear interpolation = (1 - t) * A + t * B
        //Vector3.Lerp(A, B, t);

        float oneMinusT = 1f - t;

        // //Layer 1
        // Vector3 Q = oneMinusT * A + t * B;
        // Vector3 R = oneMinusT * B + t * E;
        // Vector3 S = oneMinusT * E + t * F;

        // //Layer 2
        // Vector3 P = oneMinusT * Q + t * R;
        // Vector3 T = oneMinusT * R + t * S;

        // //Final interpolated position
        // Vector3 U = oneMinusT * P + t * T;


        //Layer 1
        Vector3 Q = Vector3.Lerp(A, B, t);
        Vector3 R = Vector3.Lerp(B, E, t);
        Vector3 S = Vector3.Lerp(E, F, t);

        //Layer 2
        Vector3 P = Vector3.Lerp(Q, R, t);
        Vector3 T = Vector3.Lerp(R, S, t);

        //Final interpolated position
        Vector3 U = Vector3.Lerp(P, T, t);

        return U;
    }
}
