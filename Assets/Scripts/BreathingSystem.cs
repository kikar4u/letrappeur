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
    //public float scaleTimeStep;
    //public float capPlayerInnerCircleMax;
    //public float capPlayerInnerCircleMin;

    #region Input
    float LeftTrigger;
    float RightTrigger;
    #endregion
    #endregion

    #region Curve
    public AnimationCurve breathCurve;
    public float speedCirclePlayer;
    #endregion

    #region Collider
    [Header("Colliders")]
    [SerializeField] Collider2D outerMarginCollider;
    [SerializeField] Collider2D innerMarginCollider;
    [SerializeField] Collider2D playerBreathCollider;
    #endregion

    #region Success
    [Header("Success Conditions")]
    public float goalAmount;
    public float pointsPerSecond;
    private float pointsAmount;
    #endregion
    [HideInInspector] public bool hasBeenInstantiated;

    [Header("Mouvement pendant la respiration")]
    public bool canWalkDuringBreathing;
    [Range(0,0.1f)]
    [HideInInspector] public float walkSpeedDuringBreathing;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.hasMovementControls = false;
        playerCircleTransform = playerCircle.GetComponent<RectTransform>();
        outerCircleTransform = donutCircle.GetComponent<RectTransform>();

        outerCircleTransform.localScale = new Vector3(breathCurve[1].value, breathCurve[1].value, 1.0f);
        StartCoroutine(BreathScaling(outerCircleSpeed));

        pointsAmount = 0f;
        hasBeenInstantiated = false;

    }

    private void Update()
    {
        LeftTrigger = Input.GetAxis("LeftTrigger");
        RightTrigger = Input.GetAxis("RightTrigger");

        // si leurs valeurs sont plus grand (donc appui de la part du joueur) on grandit le cercle
        if (LeftTrigger >= 0.1f || RightTrigger >= 0.1f)
        {
            // BUG : les conditions font buguer le système, avec les conditions le cercle n'est plus contrôlable, du moins, il bloque à une certaines distance
            playerCircleTransform.localScale -= new Vector3((LeftTrigger + RightTrigger) * Time.deltaTime * controledBreathSpeed, (LeftTrigger + RightTrigger) * Time.deltaTime * controledBreathSpeed, 0.0f);

        }
        // sinon on fait rétrécir le cercle
        else
        {
            // cap pour par que le cercle ne dépasse de l'écran si le joueur ne fait rien
            // BUG : les conditions font buguer le système, avec les conditions le cercle n'est plus contrôlable, du moins, il bloque à une certaines distance
            playerCircleTransform.localScale += new Vector3(releasedBreathSpeed * Time.deltaTime, releasedBreathSpeed * Time.deltaTime, 0.0f);

        }
        //Cap circle player
        playerCircleTransform.localScale = new Vector3(Mathf.Clamp(playerCircleTransform.localScale.x, minPlayerCircleScale, breathCurve[breathCurve.length - 1].value), Mathf.Clamp(playerCircleTransform.localScale.y, minPlayerCircleScale, breathCurve.keys[breathCurve.length - 1].value), 1.0f);

        //Si le cercle du player est dans le cercle de l'outer
        if (outerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x, playerBreathCollider.bounds.center.y, 0f))
        && !innerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x, playerBreathCollider.bounds.center.y, 0f)))
        {
            //Le joueur respire bien
            pointsAmount += pointsPerSecond / (1f / Time.deltaTime);
            if (canWalkDuringBreathing)
            {
                player.characterController.Move(Vector3.right * walkSpeedDuringBreathing);
            }
            //Debug.Log(pointsAmount);
            if (pointsAmount >= goalAmount)
            {
                player.hasMovementControls = true;
                Destroy(gameObject);
            }
        }
        else
        {
            //Le joueur respire mal
            
        }
    }

    public void CallBreathingSystem(float breathSpeed2, float scaleTimeStep2, float capPlayerInnerCircleMax2, float capPlayerInnerMin2, AnimationCurve breathCurve2, float speedCirclePlayer2)
    {

        releasedBreathSpeed = breathSpeed2;
        //capPlayerInnerCircleMax = capPlayerInnerCircleMax2;
        //capPlayerInnerCircleMin = capPlayerInnerMin2;
        breathCurve = breathCurve2;
        speedCirclePlayer = speedCirclePlayer2;

    }
    public IEnumerator BreathScaling(float Speed)
    {
        while (true)
        {
            outerCircleTransform.localScale += new Vector3(Speed * breathCurve.Evaluate(Time.time) * Time.deltaTime, Speed * breathCurve.Evaluate(Time.time) * Time.deltaTime, 1.0f);
            outerCircleTransform.localScale = new Vector3(Mathf.Clamp(outerCircleTransform.localScale.x, breathCurve[0].value, breathCurve[breathCurve.length - 1].value), Mathf.Clamp(outerCircleTransform.localScale.y, breathCurve[0].value, breathCurve[breathCurve.length - 1].value), 1.0f);
            Debug.Log(outerCircleTransform.localScale);
            yield return new WaitForSeconds(0.05f);
            if (outerCircleTransform.localScale.x >= breathCurve[breathCurve.length - 1].value || outerCircleTransform.localScale.x <= breathCurve[0].value)
            {
                Speed = -Speed;
                Debug.Log(Speed);
            }

        }
    }
}
