using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingSystem : MonoBehaviour
{
    const float VIBRATION_INTENSITY = 0.5f;

    protected Player player;
    #region Circles
    [HideInInspector] public BreathingCirclesData breathingCirclesData;
    [HideInInspector] public float minPlayerCircleScale;
    [HideInInspector] public float outerCircleSpeed;
    [Range(1, 15)]
    [HideInInspector] public float playerCircleSpeed;

    private bool ready;
    #endregion

    #region Input
    float leftTrigger;
    float rightTrigger;
    #endregion

    #region Animation
    Animator animator;
    #endregion

    #region Curve
    [HideInInspector] public BreathingUnit[] breathingUnits;
    protected BreathingUnit currentBreathing;
    [HideInInspector] public float highestValueInCurve;
    //public float speedCirclePlayer;
    #endregion

    #region Success
    [HideInInspector] public float requiredTimeSpendInsideBounds;
    float insideBoundsTimer;
    #endregion

    #region Lose
    [HideInInspector] public float requiredTimeSpendOutsideBounds;
    [HideInInspector] public float outsideBoundsTimer;
    [HideInInspector] public int requiredFailedToLose;
    //[SerializeField] AnimationCurve jiggleAnimationCurve;
    #endregion

    [Header("Mouvement pendant la respiration")]
    [HideInInspector] public bool canWalkDuringBreathing;
    [HideInInspector] public float walkSpeedDuringBreathing;

    bool stutter = false;
    bool checkingBlocked = false;

    [HideInInspector] public TriggerBreathing triggerBreathing;

    public void SetReady()
    {
        ready = true;
    }

    public void BreathingOver()
    {
        StopAllCoroutines();
        ready = false;
    }

    public void RemoveBreathingHUD()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        if (breathingCirclesData.blockThreshold == 0f)
            breathingCirclesData.blockThreshold = 0.5f;

        if (breathingCirclesData.timeCheckOffset == 0f)
            breathingCirclesData.timeCheckOffset = 0.02f;

        ready = false;
        outsideBoundsTimer = 0f;
        insideBoundsTimer = 0f;
        rightTrigger = 0f;
        leftTrigger = 0f;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.hasMovementControls = false;
        breathingCirclesData.playerCircleTransform = breathingCirclesData.playerCircle.GetComponent<RectTransform>();
        breathingCirclesData.outerCircleTransform = breathingCirclesData.donutCircle.GetComponent<RectTransform>();

        if (currentBreathing == null && breathingUnits.Length > 0)
            currentBreathing = breathingUnits[0];

        breathingCirclesData.outerCircleTransform.localScale = new Vector3(currentBreathing.breathingPattern.animationCurve[1].value, currentBreathing.breathingPattern.animationCurve[1].value, 1.0f);

        if (breathingUnits.Length == 1)
            StartCoroutine(UniqueBreathScaling(outerCircleSpeed));
        else if (breathingUnits.Length > 1)
            StartCoroutine(MultipleBreathScaling(outerCircleSpeed));
    }

    public void PopulateBreathingSystem(BreathingUnit[] _breathingUnits, int _requiredFailedToLose, float _requiredTimeSpendInsideBounds, float _requiredTimeSpendOutsideBounds, bool _canWalkDuringBreathing, float _playerCircleSpeed, TriggerBreathing _triggerBreathing, float _walkSpeedDuringBreathing = 0f)
    {
        breathingCirclesData = gameObject.GetComponent<BreathingCirclesData>();
        breathingUnits = _breathingUnits;
        triggerBreathing = _triggerBreathing;
        for (int i = 0; i < breathingUnits.Length; i++)
        {
            breathingUnits[i].breathingPattern.animationCurve.preWrapMode = _breathingUnits[i].breathingPattern.animationWrapMode;
            breathingUnits[i].breathingPattern.animationCurve.postWrapMode = _breathingUnits[i].breathingPattern.animationWrapMode;
        }
        playerCircleSpeed = _playerCircleSpeed;
        requiredTimeSpendInsideBounds = _requiredTimeSpendInsideBounds;
        requiredTimeSpendOutsideBounds = _requiredTimeSpendOutsideBounds;
        canWalkDuringBreathing = _canWalkDuringBreathing;
        requiredFailedToLose = _requiredFailedToLose;
        walkSpeedDuringBreathing = _walkSpeedDuringBreathing;
        //Récupère le point le plus haut de la courbe
        highestValueInCurve = 0f;
        minPlayerCircleScale = float.MaxValue;
        if (breathingUnits.Length == 1)
        {
            for (int i = 0; i < breathingUnits[0].breathingPattern.animationCurve.length; i++)
            {
                if (breathingUnits[0].breathingPattern.animationCurve[i].value > highestValueInCurve)
                {
                    highestValueInCurve = breathingUnits[0].breathingPattern.animationCurve[i].value;
                }
                if (breathingUnits[0].breathingPattern.animationCurve[i].value < minPlayerCircleScale)
                {
                    minPlayerCircleScale = breathingUnits[0].breathingPattern.animationCurve[i].value;
                }
            }
        }
    }

    private void Update()
    {
        if (ready)
        {
            leftTrigger = Input.GetAxis("LeftTrigger");
            rightTrigger = Input.GetAxis("RightTrigger");
            if (!stutter && !checkingBlocked)
            {
                checkingBlocked = true;
                Debug.Log(breathingCirclesData);
                StartCoroutine(CheckStutter((leftTrigger + rightTrigger) / 2));
            }
            //SmoothBreathing(LeftTrigger, RightTrigger);
            if (!stutter)
                RelativeBreathing(leftTrigger, rightTrigger);
            else
                Debug.Log("stutter");
        }
    }

    private void RelativeBreathing(float leftTriggerInput, float rightTriggerInput)
    {
        if (currentBreathing != null)
        {
            float inputsAverage = leftTriggerInput / 2 + rightTriggerInput / 2;
            float inputsToValueOnCurve = Mathf.Lerp(minPlayerCircleScale, highestValueInCurve, inputsAverage);

            Vector3 scale = new Vector3(
                Mathf.Lerp(breathingCirclesData.playerCircleTransform.localScale.x, inputsToValueOnCurve, (highestValueInCurve / minPlayerCircleScale) * playerCircleSpeed * Time.deltaTime),
                Mathf.Lerp(breathingCirclesData.playerCircleTransform.localScale.y, inputsToValueOnCurve, (highestValueInCurve / minPlayerCircleScale) * playerCircleSpeed * Time.deltaTime),
                0f);
            breathingCirclesData.playerCircleTransform.localScale = scale;
        }
    }

    protected virtual bool CheckPatternSuccess(float successTime)
    {
        if (successTime > (currentBreathing.breathingPattern.animationCurve[currentBreathing.breathingPattern.animationCurve.length - 1].time * currentBreathing.percentSuccessNeeded))
            return true;
        else
            return false;
    }

    private void UpdateUniqueBreathingSuccessCondition(bool success)
    {
        if (success)
        {
            //Le joueur respire bien
            insideBoundsTimer += Time.deltaTime;
            //On est gentil avec le joueur : si il est à l'intérieur et que son outsideTimer est supérieur à 0, il diminue 
            if (outsideBoundsTimer >= 0f)
            {
                outsideBoundsTimer -= Time.deltaTime;
                Mathf.Clamp(outsideBoundsTimer, 0f, Mathf.Infinity);
            }

            if (insideBoundsTimer >= requiredTimeSpendInsideBounds)
            {
                player.hasMovementControls = true;
                animator.SetTrigger("Over");
            }
        }
        else
        {
            //Le joueur respire mal
            outsideBoundsTimer += Time.deltaTime;
            if (outsideBoundsTimer >= requiredTimeSpendOutsideBounds)
            {
                Fader.Instance.respawnDelegate += player.Respawn;
                Fader.Instance.FadeIn();
                animator.SetTrigger("Over");
                //Debug.Log("J'ai perdu...");
            }
        }
    }

    protected virtual bool CheckCircleInBounds()
    {
        //Si le cercle du player est dans le cercle de l'outer
        if (breathingCirclesData.outerMarginCollider.bounds.Contains(new Vector3(breathingCirclesData.playerBreathCollider.bounds.max.x, breathingCirclesData.playerBreathCollider.bounds.center.y, 0f))
                && !breathingCirclesData.innerMarginCollider.bounds.Contains(new Vector3(breathingCirclesData.playerBreathCollider.bounds.max.x, breathingCirclesData.playerBreathCollider.bounds.center.y, 0f)))
        {
            if (canWalkDuringBreathing)
            {
                if (player.trapperAnim.GetCurrentState() != AnimState.PASSIVE_WALK)
                {
                    player.trapperAnim.SetAnimState(AnimState.PASSIVE_WALK);
                }
                player.WalkFollowingPath(walkSpeedDuringBreathing);
            }

            return true;
        }
        else
        {
            if (canWalkDuringBreathing)
            {
                if (player.trapperAnim.GetCurrentState() != AnimState.BREATH)
                {
                    player.trapperAnim.SetAnimState(AnimState.BREATH);
                }
            }
            return false;
        }

    }

    #region Coroutines

    public IEnumerator UniqueBreathScaling(float Speed)
    {
        while (true)
        {
            breathingCirclesData.outerCircleTransform.localScale = Vector3.Lerp(
                breathingCirclesData.outerCircleTransform.localScale,
                new Vector3(currentBreathing.breathingPattern.animationCurve.Evaluate(Time.time), currentBreathing.breathingPattern.animationCurve.Evaluate(Time.time), 1.0f),
                0.350f);
            //outerCircleTransform.localScale = new Vector3(Mathf.Clamp(outerCircleTransform.localScale.x, breathingPattern.animationCurve[0].value, breathingPattern.animationCurve[breathingPattern.animationCurve.length - 1].value), Mathf.Clamp(outerCircleTransform.localScale.y, breathingPattern.animationCurve[0].value, breathingPattern.animationCurve[breathingPattern.animationCurve.length - 1].value), 1.0f);
            yield return new WaitForSeconds(Time.deltaTime);
            UpdateUniqueBreathingSuccessCondition(CheckCircleInBounds());
        }
    }

    protected IEnumerator MultipleBreathScaling(float Speed)
    {
        float counterTime = 0f;
        float counterSuccessTime = 0f;
        int patternFailed = 0;
        for (int i = 0; i < breathingUnits.Length; i++)
        {
            if (breathingUnits[i] != currentBreathing)
                currentBreathing = breathingUnits[i];

            for (int j = 0; j < breathingUnits[i].breathingPattern.animationCurve.length; j++)
            {
                if (breathingUnits[i].breathingPattern.animationCurve[j].value > highestValueInCurve)
                {
                    highestValueInCurve = breathingUnits[i].breathingPattern.animationCurve[j].value;
                }
                if (breathingUnits[i].breathingPattern.animationCurve[j].value < minPlayerCircleScale)
                {
                    minPlayerCircleScale = breathingUnits[i].breathingPattern.animationCurve[j].value;
                }
            }

            while (counterTime <= breathingUnits[i].breathingPattern.animationCurve[breathingUnits[i].breathingPattern.animationCurve.length - 1].time)
            {
                breathingCirclesData.outerCircleTransform.localScale = Vector3.Lerp(breathingCirclesData.outerCircleTransform.localScale, new Vector3(currentBreathing.breathingPattern.animationCurve.Evaluate(counterTime), currentBreathing.breathingPattern.animationCurve.Evaluate(counterTime), 1.0f), 0.350f);
                if (CheckCircleInBounds())
                {
                    counterSuccessTime += Time.deltaTime;
                }
                yield return null;
                counterTime += Time.deltaTime;
            }
            counterTime = 0f;

            if (CheckPatternSuccess(counterSuccessTime))
            {
                //On a réussi
                Debug.Log("success");
            }
            else
            {
                //On s'est fail
                Debug.Log("fail");
                patternFailed++;
                if (patternFailed == requiredFailedToLose)
                {
                    //On a perdu
                    Fader.Instance.respawnDelegate += player.Respawn;
                    triggerBreathing.ReTrigger();
                    Fader.Instance.FadeIn();
                    animator.SetTrigger("Over");
                    break;
                }
                i--;
            }
            counterSuccessTime = 0f;
        }
        player.hasMovementControls = true;
        animator.SetTrigger("Over");
        Debug.Log(triggerBreathing);
    }

    public IEnumerator CheckStutter(float previousInput)
    {
        //Debug.Log("Previous :" + previousInput);
        yield return new WaitForSeconds(breathingCirclesData.timeCheckOffset);
        float currentTriggerInput = (Input.GetAxis("LeftTrigger") + Input.GetAxis("RightTrigger")) / 2;
        //Debug.Log("Current :" + currentTriggerInput);

        //Debug.Log(Mathf.Abs(previousInput - currentTriggerInput));

        if (Mathf.Abs(previousInput - currentTriggerInput) > breathingCirclesData.blockThreshold)
        {
            stutter = true;
            StartCoroutine(Jiggle(previousInput));
            XInputDotNetPure.GamePad.SetVibration(0, VIBRATION_INTENSITY, VIBRATION_INTENSITY);
        }
        if (!stutter)
            checkingBlocked = false;

    }

    private IEnumerator Jiggle(float previousInput)
    {
        float counter = 0f;
        while (counter < breathingCirclesData.jiggleAnimationCurve.keys[breathingCirclesData.jiggleAnimationCurve.length - 1].time)
        {
            counter += Time.deltaTime;

            breathingCirclesData.playerCircleTransform.localScale += new Vector3(breathingCirclesData.jiggleAnimationCurve.Evaluate(counter), breathingCirclesData.jiggleAnimationCurve.Evaluate(counter), 1);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        XInputDotNetPure.GamePad.SetVibration(0, 0.0f, 0.0f);
        stutter = false;
        checkingBlocked = false;
    }

    #endregion

}
