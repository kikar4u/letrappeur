using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerBreathing : MonoBehaviour
{
    bool triggered;

    #region Curve
    public BreathingUnit[] breathingUnits;
    [SerializeField] float playerCircleSpeed;
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
    [Range(0, 5f)]
    [HideInInspector] public float walkSpeedDuringBreathing;

    private void Start()
    {
        triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<TrapperAnim>().SetAnimState(AnimState.BREATH);
            triggered = true;
            GameObject prefabToInstantiate = BreathingManager.Instance.breathingPrefab;
            prefabToInstantiate.GetComponent<BreathingSystem>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
            BreathingManager.Instance.CreateBreathingCircles(prefabToInstantiate);
        }
    }

    public void Reset()
    {
        triggered = false;
    }
}
