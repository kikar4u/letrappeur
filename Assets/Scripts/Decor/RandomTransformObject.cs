using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTransformObject : MonoBehaviour
{
    public MeshFilter[] rocksPrefab;
    Vector3 initialScale;

    private void OnEnable()
    {
        initialScale = transform.localScale;
    }
    public void RandomizeRotation()
    {
        gameObject.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
    }

    public void RandomizeMesh()
    {
        if (rocksPrefab.Length > 1)
        {
            gameObject.GetComponent<MeshFilter>().mesh = rocksPrefab[Random.Range(0, rocksPrefab.Length - 1)].sharedMesh;
            GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        }
    }
    public void RandomizeScale()
    {
        if (initialScale == new Vector3(0, 0, 0))
            initialScale = transform.localScale;

        float randomScale = Random.Range(-0.2f, 0.2f);
        gameObject.transform.localScale = initialScale + new Vector3(randomScale, randomScale, randomScale);

    }
}
