using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum AnimType
{
    BLIZZARD,
    NORMAL,
    CHOPPING
}

public class TriggerBreathing : MonoBehaviour
{
    bool triggered;
    public AnimType animType;

    #region Curve
    public BreathingUnit[] breathingUnits;
    [SerializeField] float playerCircleSpeed;
    //public float speedCirclePlayer;
    #endregion

    #region Success
    [Header("Success Conditions")]
    [HideInInspector] public float requiredTimeSpendInsideBounds;
    public UnityEvent successEndEvent;
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

            //prefabToInstantiate.GetComponent<BreathingSystem>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
            GameObject breathingCircles = BreathingManager.Instance.CreateBreathingCircles(prefabToInstantiate);

            switch (animType)
            {
                case AnimType.BLIZZARD:
                    breathingCircles.AddComponent<BreathingBlizzard>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
                    break;
                case AnimType.NORMAL:
                    breathingCircles.AddComponent<BreathingNormal>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
                    break;
                case AnimType.CHOPPING:
                    other.GetComponent<TrapperAnim>().SetAnimState(AnimState.CHOP);
                    breathingCircles.AddComponent<BreathingTree>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
                    break;
            }
            BreathingManager.Instance.SetCurrentBreathing(breathingCircles.GetComponent<BreathingSystem>());
        }
    }

    public void ReTrigger()
    {
        triggered = false;
    }
}
