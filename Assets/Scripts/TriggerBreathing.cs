using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerBreathing : MonoBehaviour
{
    Player player;

    [SerializeField] GameObject breathingHUD;
    [SerializeField] GameObject HUD;
    // NOTE : On a plus besoin d'avoir ces paramètres en public , car on les donne dans TriggerBreathing à la place, ici, on s'occupe juste des sprites 
    // et des animations, de la vérification des collider et que le joueur joue bien
    [Range(0.01f, 0.1f)]
    [SerializeField] float breathSpeed;
    [SerializeField] float scaleTimeStep;
    [SerializeField] float capPlayerInnerCircleMax;
    [SerializeField] float capPlayerInnerCircleMin;
    //Vector3 originalScale;

    bool hasBeenInstantiated;
    #region Input
    float LeftTrigger;
    float RightTrigger;
    #endregion

    [SerializeField] AnimationCurve breathCurve;
    [SerializeField] float speedCirclePlayer;
    // Start is called before the first frame update
    void Start()
    {
        hasBeenInstantiated = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        BreathingSystem breathingVar = breathingHUD.GetComponent<BreathingSystem>();
        if (other.tag == "Player" && !hasBeenInstantiated)
        {
            // on set toutes les variables données en paramètres par la triggerbox au script de respiration
            breathingVar.breathSpeed = breathSpeed;
            breathingVar.scaleTimeStep = scaleTimeStep;
            breathingVar.capPlayerInnerCircleMax = capPlayerInnerCircleMax;
            breathingVar.capPlayerInnerCircleMin = capPlayerInnerCircleMin;
            breathingVar.breathCurve = breathCurve;
            breathingVar.speedCirclePlayer = speedCirclePlayer;
            // on appelle la fonction avec les paramètres mis dans le trigger
            // breathingHUD.GetComponent<BreathingSystem>().CallBreathingSystem(breathSpeed, scaleTimeStep, capPlayerInnerCircleMax, capPlayerInnerCircleMin, breathCurve, speedCirclePlayer);
            Instantiate(breathingHUD, HUD.transform);
            hasBeenInstantiated = true;
            // on appelle la fonction, avec les paramètres pour ce trigger
            Debug.Log("C'est le joueur qui passe par là");
            //Instantiate(BreathingHUD, HUD.transform);
        }

    }
}
