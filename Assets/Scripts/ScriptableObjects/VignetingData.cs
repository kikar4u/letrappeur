using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VignetingData
{
    [Range(0f, 1f)]
    public float maxIntensity;
    [Range(0f, 0.1f)]
    public float offset;
    [Range(0.5f, 5f)]
    public float periodTime;

    [HideInInspector] public float currentAverage;

}
