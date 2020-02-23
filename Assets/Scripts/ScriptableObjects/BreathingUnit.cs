using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ce script contient les données relative à une unité de mécanique de respiration
[System.Serializable]
public class BreathingUnit
{
    public BreathingPattern breathingPattern;
    [Range(0, 1)]
    [Tooltip("Entre 0 et 1, cette valeur correspond au pourcentage de réussite de ce pattern pour être validé")]
    public float percentSuccessNeeded;
}
