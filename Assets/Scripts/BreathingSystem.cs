using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingSystem : MonoBehaviour
{
    Player player;
    #region Circles
    // cercle player
    [SerializeField]Image innerCircle;
    RectTransform innerCircleTransform;
    // cercle avatar
    [SerializeField]Image outerCircle;
    RectTransform outerCircleTransform;
    [Range(0.01f,0.1f)]
    [SerializeField] float breathSpeed;
    [SerializeField] float scaleTimeStep;
    Vector3 originalScale;
    #region Input
    float LeftTrigger;
    float RightTrigger;
    #endregion
    [SerializeField] AnimationCurve breathCurve;
    [SerializeField] float speedCirclePlayer;
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

        //Debug.Log(" Breath courb : " + breathCurve);

        StartCoroutine(BreathScaling(breathSpeed));
    }

    private void Update()
    {   // on récupère les valeurs des deux gachettes
        LeftTrigger = Input.GetAxis("LeftTrigger");
        RightTrigger = Input.GetAxis("RightTrigger");
        //Debug.Log("right trigger : " + RightTrigger);
        //Debug.Log("Left trigger" + LeftTrigger);
        // si leurs valeurs sont plus grand (donc appui de la part du joueur) on grandit le cercle
        if(LeftTrigger >= 0.1f || RightTrigger >= 0.1f)
        {
            // NOTE : J'ai mis un 2 pour augmenter la vitesse, mais il doit y avoir moyen de faire autrement
            innerCircleTransform.localScale += new Vector3((LeftTrigger + RightTrigger) * Time.deltaTime * 2, (LeftTrigger + RightTrigger) * Time.deltaTime * 2, 0.0f);
        }
        // sinon on fait rétrécir le cercle
        else
        {
            innerCircleTransform.localScale -= new Vector3((speedCirclePlayer) * Time.deltaTime, (speedCirclePlayer) * Time.deltaTime, 0.0f);
        }
        
        //Si le cercle du player est dans le cercle de l'outer
        if (outerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x,playerBreathCollider.bounds.center.y, 0f))
         && !innerMarginCollider.bounds.Contains(new Vector3(playerBreathCollider.bounds.max.x,playerBreathCollider.bounds.center.y, 0f))){
            //Le joueur respire bien
            Debug.Log("Tu respires bien, gg !");
        }else{
            //Le joueur respire mal
            Debug.Log("Tu respires mal connard !");
        }

    }

    IEnumerator BreathScaling(float Speed){
        while(true){
            
            outerCircleTransform.localScale += new Vector3(Speed * breathCurve.Evaluate(Time.time), Speed * breathCurve.Evaluate(Time.time), 0.0f);
            
            yield return new WaitForSeconds(Time.deltaTime);

            if(outerCircleTransform.localScale.x >= 3.0f)
            {
                Speed = -Speed;
            }

            if(outerCircleTransform.localScale.x <= originalScale.x)
            {
                Speed = -Speed;
            }
        }
    }
}
