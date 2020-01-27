using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTransformObject : MonoBehaviour
{
    public MeshFilter[] rocksPrefab;
    public void RandomizeRotation()
    {
        gameObject.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
    }

    public void RandomizeMesh()
    {
        if (rocksPrefab.Length > 1)
            gameObject.GetComponent<MeshFilter>().mesh = rocksPrefab[Random.Range(0, rocksPrefab.Length - 1)].sharedMesh;
    }
    public void RandomizeScale()
    {
        float randomScale = Random.Range(0.8f, 1.2f);
        gameObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }
}
