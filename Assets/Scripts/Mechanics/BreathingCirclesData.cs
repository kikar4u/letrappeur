using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BreathingCirclesData : MonoBehaviour
{

    public GameObject playerCircle;
    public RectTransform playerCircleTransform;
    // cercle avatar
    public GameObject donutCircle;
    public RectTransform outerCircleTransform;

    public ParticleSystem particles;

    #region Collider
    [Header("Colliders")]
    public Collider outerMarginCollider;
    public Collider innerMarginCollider;
    public Collider playerBreathCollider;
    #endregion

    #region Stutter
    public AnimationCurve jiggleAnimationCurve;

    [Header("Stutter")]
    [Range(0f, 1f)]
    public float blockThreshold;
    [Range(0f, 1f)]
    public float timeCheckOffset;
    #endregion
}
