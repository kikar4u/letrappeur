using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerBreathing : MonoBehaviour
{
    bool triggered;
    //[SerializeField] BreathingSystem breathingSystem;

    /*[Range(0.2f, 2f)]
    public float releasedBreathSpeed;
    [Range(0.2f, 2f)]
    public float controledBreathSpeed;*/
    //[Range(0.2f, 2f)]
    //public float outerCircleSpeed;

    #region Curve
    public BreathingUnit[] breathingUnits;
    //public float speedCirclePlayer;
    #endregion

    #region Success
    [Header("Success Conditions")]
    [HideInInspector] public float requiredTimeSpendInsideBounds;
    #endregion

    #region Lose
    [Header("Lose Conditions")]
    [Tooltip("Au bout de X secondes, le joueur aura raté.")]
    [HideInInspector] public float requiredTimeSpendOutsideBounds;
    [Range(1, 5)]
    [HideInInspector] public int requiredFailedToLose;
    #endregion

    [Header("Mouvement pendant la respiration")]
    public bool canWalkDuringBreathing;
    [Range(0, 180f)]
    [HideInInspector] public float walkSpeedDuringBreathing;

    private void Start()
    {
        triggered = false;
        //breathingSystem.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject.tag == "Player")
        {
            //breathingSystem.gameObject.SetActive(true);
            triggered = true;
            GameObject prefabToInstantiate = BreathingManager.Instance.breathingPrefab;
            prefabToInstantiate.GetComponent<BreathingSystem>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, walkSpeedDuringBreathing);
            BreathingManager.Instance.CreateBreathingCircles(prefabToInstantiate);
        }
    }
}
