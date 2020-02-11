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
    [SerializeField] private float minPlayerCircleScale;
    // cercle avatar
    [SerializeField] GameObject donutCircle;
    RectTransform outerCircleTransform;

    [Range(0.2f, 2f)]
    public float releasedBreathSpeed;
    [Range(0.2f, 2f)]
    public float controledBreathSpeed;
    [Range(0.2f, 2f)]
    public float outerCircleSpeed;

    #endregion
    #region Input
    float LeftTrigger;
    float RightTrigger;
    #endregion

    #region Curve
    public BreathingPattern breathingPattern;
    private float highestValueInCurve;
    //public float speedCirclePlayer;
    #endregion

    #region Collider
    [Header("Colliders")]
    [SerializeField] Collider2D outerMarginCollider;
    [SerializeField] Collider2D innerMarginCollider;
    [SerializeField] Collider2D playerBreathCollider;
    #endregion

    #region Success
    [Header("Success Conditions")]
    public float requiredTimeSpendInsideBounds;
    public float pointsPerSecond;
    private float pointsAmount;
    #endregion

    #region Lose
    [Header("Lose Conditions")]
    [Tooltip("Au bout de X secondes, le joueur aura raté.")]
    public float requiredTimeSpendOutsideBounds;
    private float outsideBoundsTimer;
    #endregion

    [Header("Mouvement pendant la respiration")]
    public bool canWalkDuringBreathing;
    [Range(0, 180f)]
    [HideInInspector] public float walkSpeedDuringBreathing;

    void Start()
    {
        outsideBoundsTimer = 0f;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.hasMovementControls = false;
        playerCircleTransform = playerCircle.GetComponent<RectTransform>();
        outerCircleTransform = donutCircle.GetComponent<RectTransform>();

        outerCircleTransform.localScale = new Vector3(breathingPattern.animationCurve[1].value, breathingPattern.animationCurve[1].value, 1.0f);
        StartCoroutine(BreathScaling(outerCircleSpeed));

        pointsAmount = 0f;

        breathingPattern.animationCurve.preWrapMode = breathingPattern.animationWrapMode;
        breathingPattern.animationCurve.postWrapMode = breathingPattern.animationWrapMode;

        highestValueInCurve = 0f;
        //Récupère le point le plus haut de la courbe
        for (int i = 0; i < breathingPattern.animationCurve.length; i++)
        {
            if (breathingPattern.animationCurve[i].value > highestValueInCurve)
            {
                highestValueInCurve = breathingPattern.animationCurve[i].value;
            }
        }
    }

    private void Update()
    {

        float LeftTrigger = Input.GetAxis("LeftTrigger");
        float RightTrigger = Input.GetAxis("RightTrigger");

        SmoothBreathing(LeftTrigger, RightTrigger);
        //RelativeBreathing(LeftTrigger, RightTrigger);
        CheckCircleInBounds();
    }

    private void RelativeBreathing(float leftTriggerInput, float rightTriggerInput)
    {
        float lerpValue = Mathf.Lerp(breathingPattern.animationCurve[0].value, breathingPattern.animationCurve[breathingPattern.animationCurve.length - 1].value, leftTriggerInput / 2 + rightTriggerInput / 2);

        Vector3 scale = new Vector3(Mathf.Lerp(playerCircleTransform.localScale.x, lerpValue, 0.1f), Mathf.Lerp(playerCircleTransform.localScale.y, lerpValue, 0.1f), 0f);
        playerCircleTransform.localScale = scale;
    }

    private void SmoothBreathing(float leftTriggerInput, float rightTriggerInput)
    {
        // si leurs valeurs sont plus grand (donc appui de la part du joueur) on grandit le cercle
        if (leftTriggerInput >= 0.1f || rightTriggerInput >= 0.1f)
        {
            // BUG : les conditions font buguer le système, avec les conditions le cercle n'est plus contrôlable, du moins, il bloque à une certaines distance
            playerCircleTransform.localScale += new Vector3((leftTriggerInput + rightTriggerInput) * Time.deltaTime * controledBreathSpeed, (leftTriggerInput + rightTriggerInput) * Time.deltaTime * controledBreathSpeed, 0.0f);
        }
        // sinon on fait rétrécir le cercle
        else
        {
            // cap pour par que le cercle ne dépasse de l'écran si le joueur ne fait rien
            if (playerCircleTransform.localScale.x > outerCircleTransform.localScale.x /*&& !CheckCircleInBounds()*/)
            {
                //playerCircleTransform.localScale -= new Vector3(releasedBreathSpeed * 2 * Time.deltaTime, releasedBreathSpeed * 2 * Time.deltaTime, 0.0f);
                playerCircleTransform.localScale = Vector3.Lerp(playerCircleTransform.localScale, new Vector3(breathingPattern.animationCurve.Evaluate(Time.time), breathingPattern.animationCurve.Evaluate(Time.time), 0f), 0.250f);
            }
            //playerCircleTransform.localScale -= new Vector3(releasedBreathSpeed * Time.deltaTime, releasedBreathSpeed * Time.deltaTime, 0.0f);
        }
        //Cap circle player
        playerCircleTransform.localScale = new Vector3(Mathf.Clamp(playerCircleTransform.localScale.x, minPlayerCircleScale, highestValueInCurve), Mathf.Clamp(playerCircleTransform.localScale.y, minPlayerCircleScale, highestValueInCurve), 1.0f);

    }

    private bool CheckCircleInBounds()
    {
        //Si le cercle du player est dans le cercle de l'outer
        if (outerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x, playerBreathCollider.bounds.center.y, 0f))
                && !innerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x, playerBreathCollider.bounds.center.y, 0f)))
        {
            //Le joueur respire bien
            pointsAmount += pointsPerSecond / (1f / Time.deltaTime);

            //On est gentil avec le joueur : si il est à l'intérieur et que son outsideTimer est supérieur à 0, il diminue 
            if (outsideBoundsTimer >= 0f)
            {
                outsideBoundsTimer -= Time.deltaTime;
                Mathf.Clamp(outsideBoundsTimer, 0f, Mathf.Infinity);
            }
            if (canWalkDuringBreathing)
            {
                player.WalkFollowingPath(walkSpeedDuringBreathing);
            }
            if (pointsAmount >= requiredTimeSpendInsideBounds)
            {
                player.hasMovementControls = true;
                Destroy(gameObject, 0.01f);
            }
            return true;
        }
        else
        {
            //Le joueur respire mal
            outsideBoundsTimer += Time.deltaTime;
            if (outsideBoundsTimer <= requiredTimeSpendOutsideBounds)
            {
                //Debug.Log(outsideBoundsTimer);
            }
            if (outsideBoundsTimer >= requiredTimeSpendOutsideBounds)
            {
                //Debug.Log("J'ai perdu...");
            }
            return false;
        }
    }

    public IEnumerator BreathScaling(float Speed)
    {
        while (true)
        {
            outerCircleTransform.localScale = Vector3.Lerp(outerCircleTransform.localScale, new Vector3(breathingPattern.animationCurve.Evaluate(Time.time), breathingPattern.animationCurve.Evaluate(Time.time), 1.0f), 0.350f);
            //outerCircleTransform.localScale = new Vector3(Mathf.Clamp(outerCircleTransform.localScale.x, breathingPattern.animationCurve[0].value, breathingPattern.animationCurve[breathingPattern.animationCurve.length - 1].value), Mathf.Clamp(outerCircleTransform.localScale.y, breathingPattern.animationCurve[0].value, breathingPattern.animationCurve[breathingPattern.animationCurve.length - 1].value), 1.0f);
            yield return new WaitForSeconds(Time.deltaTime);
            if (outerCircleTransform.localScale.x >= breathingPattern.animationCurve[breathingPattern.animationCurve.length - 1].value || outerCircleTransform.localScale.x <= breathingPattern.animationCurve[0].value)
            {
                //Speed = -Speed;
                //Debug.Log(Speed);
            }
        }
    }
}
