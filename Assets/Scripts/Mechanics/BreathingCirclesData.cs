using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class BreathingCirclesData
{
    // cercle player
    [SerializeField] Image playerCircle;
    RectTransform playerCircleTransform;
    // cercle avatar
    [SerializeField] GameObject donutCircle;
    RectTransform outerCircleTransform;
}
