using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public int segments = 50;
    public float xRadius;
    public float yRadius;
    public float width;
    LineRenderer line;

    SphereCollider sphereCollider;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();

        line.startWidth = width;

        line.positionCount = segments + 1;
        sphereCollider.radius = (xRadius + yRadius) / 2;

        line.useWorldSpace = false;
        CreatePoints();
        Debug.Log("CreatePoints");
    }

    public void CreatePoints()
    {
        float x, y;
        float z = 0f;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xRadius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yRadius;

            line.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);

        }
    }


}
