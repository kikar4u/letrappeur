using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BreathingCirclesData : MonoBehaviour
{
    // cercle player
    public Image playerCircle;
    public RectTransform playerCircleTransform;
    // cercle avatar
    public GameObject donutCircle;
    public RectTransform outerCircleTransform;

    #region Collider
    [Header("Colliders")]
    public Collider2D outerMarginCollider;
    public Collider2D innerMarginCollider;
    public Collider2D playerBreathCollider;
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
