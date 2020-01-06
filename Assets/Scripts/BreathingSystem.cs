using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingSystem : MonoBehaviour
{
    Player player;
    #region Circles
    // cercle player
    [SerializeField] Image innerCircle;
    RectTransform innerCircleTransform;
    // cercle avatar
    [SerializeField] Image outerCircle;
    RectTransform outerCircleTransform;
    [Range(0.01f,0.1f)]
    public float breathSpeed;
    public float scaleTimeStep;
    public float capPlayerInnerCircleMax;
    public float capPlayerInnerCircleMin;
    Vector3 originalScale;
    #region Input
    float LeftTrigger;
    float RightTrigger;
    #endregion
    public AnimationCurve breathCurve;
    public float speedCirclePlayer;
    #endregion

    #region Collider
    [Header("Colliders")]
    [SerializeField]Collider2D outerMarginCollider;
    [SerializeField]Collider2D innerMarginCollider;
    [SerializeField]Collider2D playerBreathCollider;
    #endregion
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        innerCircleTransform = innerCircle.GetComponent<RectTransform>();
        outerCircleTransform = outerCircle.GetComponent<RectTransform>();

        originalScale = outerCircleTransform.localScale;
        StartCoroutine(BreathScaling(breathSpeed));
        //Debug.Log(" Breath courb : " + breathCurve);


    }

    private void Update()
    {
        LeftTrigger = Input.GetAxis("LeftTrigger");
        RightTrigger = Input.GetAxis("RightTrigger");
        //Debug.Log("right trigger : " + RightTrigger);
        //Debug.Log("Left trigger" + LeftTrigger);
        // si leurs valeurs sont plus grand (donc appui de la part du joueur) on grandit le cercle
        if (LeftTrigger >= 0.1f || RightTrigger >= 0.1f)
        {

            // NOTE : J'ai mis un 2 pour augmenter la vitesse, mais il doit y avoir moyen de faire autrement
            // BUG : les conditions font buguer le système, avec les conditions le cercle n'est plus contrôlable, du moins, il bloque à une certaines distance
            //if (innerCircle.rectTransform.localScale.x >= capPlayerInnerCircleMin)
            //{
            innerCircleTransform.localScale -= new Vector3((LeftTrigger + RightTrigger) * Time.deltaTime * 2, (LeftTrigger + RightTrigger) * Time.deltaTime * 2, 0.0f);
            //}


        }
        // sinon on fait rétrécir le cercle
        else
        {
            // cap pour par que le cercle ne dépasse de l'écran si le joueur ne fait rien
            // BUG : les conditions font buguer le système, avec les conditions le cercle n'est plus contrôlable, du moins, il bloque à une certaines distance
            //if (innerCircle.rectTransform.localScale.x <= capPlayerInnerCircleMax)
            //{
            //Debug.Log("fjqdsi" + innerCircleTransform.localScale.x);
            innerCircleTransform.localScale += new Vector3((speedCirclePlayer) * Time.deltaTime, (speedCirclePlayer) * Time.deltaTime, 0.0f);
            //}

        }

        //Si le cercle du player est dans le cercle de l'outer
        if (outerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x, playerBreathCollider.bounds.center.y, 0f))
         && !innerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x, playerBreathCollider.bounds.center.y, 0f)))
        {
            //Le joueur respire bien
            Debug.Log("Tu respires bien, gg !");
        }
        else
        {
            //Le joueur respire mal
            Debug.Log("Tu respires mal connard !");
        }
        Debug.Log("shit here we go again");

    }

    public void CallBreathingSystem(float breathSpeed2, float scaleTimeStep2, float capPlayerInnerCircleMax2, float capPlayerInnerMin2, AnimationCurve breathCurve2, float speedCirclePlayer2)
    {

        breathSpeed = breathSpeed2;
        capPlayerInnerCircleMax = capPlayerInnerCircleMax2;
        capPlayerInnerCircleMin = capPlayerInnerMin2;
        breathCurve = breathCurve2;
        speedCirclePlayer = speedCirclePlayer2;

    }
    public IEnumerator BreathScaling(float Speed){
        Debug.Log("breathSpeed" + breathSpeed);
        Debug.Log("BreathCurve" + breathCurve);
        Debug.Log("Speed circle player" + speedCirclePlayer);
        while (true){
            Debug.Log("outerscaleBefore" + outerCircleTransform.localScale);
            outerCircleTransform.localScale += new Vector3(Speed * breathCurve.Evaluate(Time.time), Speed * breathCurve.Evaluate(Time.time), 0.0f);
            Debug.Log("outerscale" + outerCircleTransform.localScale);
            yield return new WaitForSeconds(0.01f);
            if (outerCircleTransform.localScale.x >= 1.5f)
            {
                Speed = -Speed;
                Debug.Log("shit here=");
            }

            if(outerCircleTransform.localScale.x <= originalScale.x)
            {
                Speed = -Speed;
            }


        }
    }
}
