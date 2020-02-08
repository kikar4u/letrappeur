using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "Breathing")]
public class BreathingPattern : ScriptableObject
{
    public AnimationCurve animationCurve;
    public WrapMode animationWrapMode;

}
