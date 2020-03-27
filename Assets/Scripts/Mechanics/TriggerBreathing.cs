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

    //[HideInInspector] public DestroyableTree tree;

    #region Juice/Polish
    public bool doCameraShake;
    [HideInInspector] public float shakeIntensity;
    public VignetingData vignetingData;
    #endregion

    private void Start()
    {
        triggered = false;

        if (breathingUnits.Length > 1)
        {
            vignetingData.currentAverage = vignetingData.initialIntensity;
        }
        else
        {
            vignetingData.currentAverage = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject.tag == "Player")
        {
            PostProcessManager.Instance.SetVignetingData(vignetingData);
            PostProcessManager.Instance.StartVigneting();

            other.gameObject.GetComponent<Player>().trapperAnim.SetAnimState(AnimState.BREATH);
            triggered = true;
            AudioClip startBreathingClip;
            switch (animType)
            {
                case AnimType.BLIZZARD:
                    startBreathingClip = _MGR_SoundDesign.Instance.GetSpecificClip("StartPanic");
                    break;
                case AnimType.NORMAL:
                    startBreathingClip = _MGR_SoundDesign.Instance.GetSpecificClip("StartPanic");
                    break;
                case AnimType.CHOPPING:
                    other.gameObject.GetComponent<Player>().trapperAnim.SetAnimState(AnimState.CHOP);
                    startBreathingClip = _MGR_SoundDesign.Instance.GetSpecificClip("StartPanic");
                    break;
                default:
                    startBreathingClip = _MGR_SoundDesign.Instance.GetSpecificClip("StartPanic");
                    break;
            }
            other.GetComponent<Player>().audioSource.loop = false;
            _MGR_SoundDesign.Instance.PlaySpecificSound(startBreathingClip, other.gameObject.GetComponent<Player>().audioSource);
            StartCoroutine(InstantationTimeOffset(startBreathingClip.length));

            if (doCameraShake)
            {
                Camera.main.GetComponent<CameraShakin>().Shake(shakeIntensity);
            }
        }
    }

    IEnumerator InstantationTimeOffset(float duration)
    {
        yield return new WaitForSeconds(duration);
        GameObject prefabToInstantiate = BreathingManager.Instance.breathingPrefab;

        //prefabToInstantiate.GetComponent<BreathingSystem>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
        GameObject breathingCircles = BreathingManager.Instance.CreateBreathingCircles(prefabToInstantiate);
        AudioClip duringBreathingClip;
        switch (animType)
        {
            case AnimType.BLIZZARD:
                breathingCircles.AddComponent<BreathingBlizzard>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
                duringBreathingClip = _MGR_SoundDesign.Instance.GetSpecificClip("DuringPanic");
                break;
            case AnimType.NORMAL:
                breathingCircles.AddComponent<BreathingNormal>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
                duringBreathingClip = _MGR_SoundDesign.Instance.GetSpecificClip("DuringPanic");
                break;
            case AnimType.CHOPPING:
                breathingCircles.AddComponent<BreathingTree>().PopulateBreathingSystem(breathingUnits, requiredFailedToLose, requiredTimeSpendInsideBounds, requiredTimeSpendOutsideBounds, canWalkDuringBreathing, playerCircleSpeed, this, walkSpeedDuringBreathing);
                duringBreathingClip = _MGR_SoundDesign.Instance.GetSpecificClip("DuringPanic");
                break;
            default:
                duringBreathingClip = _MGR_SoundDesign.Instance.GetSpecificClip("DuringPanic");
                break;
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().audioSource.loop = true;
        BreathingManager.Instance.SetCurrentBreathing(breathingCircles.GetComponent<BreathingSystem>());
        _MGR_SoundDesign.Instance.PlaySpecificSound(duringBreathingClip, GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().audioSource);

    }

    public void ReTrigger()
    {
        triggered = false;
    }
}
