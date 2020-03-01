using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingSystem : MonoBehaviour
{
    Player player;
    #region Circles
    // cercle player
    [SerializeField] Image playerCircle;
    RectTransform playerCircleTransform;
    [HideInInspector] public float minPlayerCircleScale;
    // cercle avatar
    [SerializeField] GameObject donutCircle;
    RectTransform outerCircleTransform;
    [HideInInspector] public float outerCircleSpeed;

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
    BreathingUnit currentBreathing;
    [HideInInspector] public float highestValueInCurve;
    //public float speedCirclePlayer;
    #endregion

    #region Collider
    [Header("Colliders")]
    [SerializeField] Collider2D outerMarginCollider;
    [SerializeField] Collider2D innerMarginCollider;
    [SerializeField] Collider2D playerBreathCollider;
    #endregion

    #region Success
    [HideInInspector] public float requiredTimeSpendInsideBounds;
    float insideBoundsTimer;
    #endregion

    #region Lose
    [HideInInspector] public float requiredTimeSpendOutsideBounds;
    [HideInInspector] public float outsideBoundsTimer;
    [HideInInspector] public int requiredFailedToLose;
    [SerializeField] AnimationCurve jiggleAnimationCurve;
    #endregion

    [Header("Mouvement pendant la respiration")]
    bool canWalkDuringBreathing;
    float walkSpeedDuringBreathing;

    bool stutter = false;
    bool checkingBlocked = false;
    float blockThreshold = 0.5f;

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
        ready = false;
        outsideBoundsTimer = 0f;
        insideBoundsTimer = 0f;
        rightTrigger = 0f;
        leftTrigger = 0f;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.hasMovementControls = false;
        playerCircleTransform = playerCircle.GetComponent<RectTransform>();
        outerCircleTransform = donutCircle.GetComponent<RectTransform>();
        animator = GetComponent<Animator>();

        if (currentBreathing == null && breathingUnits.Length > 0)
            currentBreathing = breathingUnits[0];

        outerCircleTransform.localScale = new Vector3(currentBreathing.breathingPattern.animationCurve[1].value, currentBreathing.breathingPattern.animationCurve[1].value, 1.0f);

        if (breathingUnits.Length == 1)
            StartCoroutine(UniqueBreathScaling(outerCircleSpeed));
        else if (breathingUnits.Length > 1)
            StartCoroutine(MultipleBreathScaling(outerCircleSpeed));
    }

    public void PopulateBreathingSystem(BreathingUnit[] _breathingUnits, int _requiredFailedToLose, float _requiredTimeSpendInsideBounds, float _requiredTimeSpendOutsideBounds, bool _canWalkDuringBreathing, float _walkSpeedDuringBreathing = 0f)
    {
        breathingUnits = _breathingUnits;
        for (int i = 0; i < breathingUnits.Length; i++)
        {
            breathingUnits[i].breathingPattern.animationCurve.preWrapMode = _breathingUnits[i].breathingPattern.animationWrapMode;
            breathingUnits[i].breathingPattern.animationCurve.postWrapMode = _breathingUnits[i].breathingPattern.animationWrapMode;
        }
        requiredTimeSpendInsideBounds = _requiredTimeSpendInsideBounds;
        requiredTimeSpendOutsideBounds = _requiredTimeSpendOutsideBounds;
        canWalkDuringBreathing = _canWalkDuringBreathing;
        requiredFailedToLose = _requiredFailedToLose;

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
                Mathf.Lerp(playerCircleTransform.localScale.x, inputsToValueOnCurve, (highestValueInCurve / minPlayerCircleScale) / 2 * Time.deltaTime),
                Mathf.Lerp(playerCircleTransform.localScale.y, inputsToValueOnCurve, (highestValueInCurve / minPlayerCircleScale) / 2 * Time.deltaTime),
                0f);
            playerCircleTransform.localScale = scale;
        }
    }

    //private void SmoothBreathing(float leftTriggerInput, float rightTriggerInput)
    //{
    //    // si leurs valeurs sont plus grand (donc appui de la part du joueur) on grandit le cercle
    //    if (leftTriggerInput >= 0.1f || rightTriggerInput >= 0.1f)
    //    {
    //        // BUG : les conditions font buguer le système, avec les conditions le cercle n'est plus contrôlable, du moins, il bloque à une certaines distance
    //        playerCircleTransform.localScale += new Vector3((leftTriggerInput + rightTriggerInput) * Time.deltaTime * controledBreathSpeed, (leftTriggerInput + rightTriggerInput) * Time.deltaTime * controledBreathSpeed, 0.0f);
    //    }
    //    // sinon on fait rétrécir le cercle
    //    else
    //    {
    //        // cap pour par que le cercle ne dépasse de l'écran si le joueur ne fait rien
    //        if (playerCircleTransform.localScale.x > outerCircleTransform.localScale.x /*&& !CheckCircleInBounds()*/)
    //        {
    //            //playerCircleTransform.localScale -= new Vector3(releasedBreathSpeed * 2 * Time.deltaTime, releasedBreathSpeed * 2 * Time.deltaTime, 0.0f);
    //            playerCircleTransform.localScale = Vector3.Lerp(playerCircleTransform.localScale, new Vector3(currentBreathingPattern.animationCurve.Evaluate(Time.time), currentBreathingPattern.animationCurve.Evaluate(Time.time), 0f), 0.250f);
    //        }
    //        //playerCircleTransform.localScale -= new Vector3(releasedBreathSpeed * Time.deltaTime, releasedBreathSpeed * Time.deltaTime, 0.0f);
    //    }
    //    //Cap circle player
    //    playerCircleTransform.localScale = new Vector3(Mathf.Clamp(playerCircleTransform.localScale.x, minPlayerCircleScale, highestValueInCurve), Mathf.Clamp(playerCircleTransform.localScale.y, minPlayerCircleScale, highestValueInCurve), 1.0f);
    //}

    private bool CheckPatternSuccess(float successTime)
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
                Fader.Instance.FadeIn();
                animator.SetTrigger("Over");
                //Debug.Log("J'ai perdu...");
            }
        }
    }

    private bool CheckCircleInBounds()
    {
        //Si le cercle du player est dans le cercle de l'outer
        if (outerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x, playerBreathCollider.bounds.center.y, 0f))
                && !innerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x, playerBreathCollider.bounds.center.y, 0f)))
        {
            if (canWalkDuringBreathing)
            {
                player.WalkFollowingPath(walkSpeedDuringBreathing);
            }

            return true;
        }
        else
            return false;
    }

    #region Coroutines

    public IEnumerator UniqueBreathScaling(float Speed)
    {
        while (true)
        {
            outerCircleTransform.localScale = Vector3.Lerp(outerCircleTransform.localScale, new Vector3(currentBreathing.breathingPattern.animationCurve.Evaluate(Time.time), currentBreathing.breathingPattern.animationCurve.Evaluate(Time.time), 1.0f), 0.350f);
            //outerCircleTransform.localScale = new Vector3(Mathf.Clamp(outerCircleTransform.localScale.x, breathingPattern.animationCurve[0].value, breathingPattern.animationCurve[breathingPattern.animationCurve.length - 1].value), Mathf.Clamp(outerCircleTransform.localScale.y, breathingPattern.animationCurve[0].value, breathingPattern.animationCurve[breathingPattern.animationCurve.length - 1].value), 1.0f);
            yield return new WaitForSeconds(Time.deltaTime);
            UpdateUniqueBreathingSuccessCondition(CheckCircleInBounds());
        }
    }

    public IEnumerator MultipleBreathScaling(float Speed)
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
                outerCircleTransform.localScale = Vector3.Lerp(outerCircleTransform.localScale, new Vector3(currentBreathing.breathingPattern.animationCurve.Evaluate(counterTime), currentBreathing.breathingPattern.animationCurve.Evaluate(counterTime), 1.0f), 0.350f);
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
                    Fader.Instance.FadeIn();
                    player.Respawn();
                    Destroy(gameObject);
                    break;
                }
                i--;
            }
            counterSuccessTime = 0f;
        }
        player.hasMovementControls = true;
        animator.SetTrigger("Over");
    }

    private IEnumerator CheckStutter(float previousInput)
    {
        //Debug.Log("Previous :" + previousInput);
        yield return new WaitForSeconds(0.02f);
        float currentTriggerInput = (Input.GetAxis("LeftTrigger") + Input.GetAxis("RightTrigger")) / 2;
        //Debug.Log("Current :" + currentTriggerInput);

        //Debug.Log(Mathf.Abs(previousInput - currentTriggerInput));

        if (Mathf.Abs(previousInput - currentTriggerInput) > blockThreshold)
        {
            stutter = true;
            StartCoroutine(Jiggle(previousInput));
            XInputDotNetPure.GamePad.SetVibration(0, 5f, 5f);
        }
        if (!stutter)
            checkingBlocked = false;
        //else
        //{
        //    yield return new WaitForSeconds(1f);
        //   stutter = false;
        //}
    }

    private IEnumerator Jiggle(float previousInput)
    {
        float counter = 0f;
        while (counter < jiggleAnimationCurve.keys[jiggleAnimationCurve.length - 1].time)
        {
            counter += Time.deltaTime;

            //float lerpValue = Mathf.Lerp(currentBreathing.breathingPattern.animationCurve[0].value, highestValueInCurve, previousInput);

            //Vector3 scale = new Vector3(Mathf.Lerp(playerCircleTransform.localScale.x, lerpValue, Mathf.InverseLerp(minPlayerCircleScale / highestValueInCurve, 1, lerpValue) * Time.deltaTime * 2), Mathf.Lerp(playerCircleTransform.localScale.y, lerpValue, Mathf.InverseLerp(minPlayerCircleScale / highestValueInCurve, 1, lerpValue) * Time.deltaTime * 2), 0f);
            //playerCircleTransform.localScale = scale;
            playerCircleTransform.localScale += new Vector3(jiggleAnimationCurve.Evaluate(counter), jiggleAnimationCurve.Evaluate(counter), 1);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        XInputDotNetPure.GamePad.SetVibration(0, 0.0f, 0.0f);
        stutter = false;
        checkingBlocked = false;
    }

    #endregion

}
