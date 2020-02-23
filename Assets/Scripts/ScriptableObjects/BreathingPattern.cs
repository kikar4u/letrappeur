using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ce script contient les données inhérentes à un pattern de respiration
[CreateAssetMenu(fileName = "Pattern", menuName = "Breathing")]
public class BreathingPattern : ScriptableObject
{
    public AnimationCurve animationCurve;
    public WrapMode animationWrapMode;

}
