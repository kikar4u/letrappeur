using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BreathingCirclesImageData : BreathingCirclesData
{
    // cercle player
    public Image playerCircle;

    #region Collider
    [Header("Colliders")]
    public Collider2D outerMarginCollider;
    public Collider2D innerMarginCollider;
    public Collider2D playerBreathCollider;
    #endregion
}
