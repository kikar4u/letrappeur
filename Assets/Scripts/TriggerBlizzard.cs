using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBlizzard : MonoBehaviour
{
    public GameObject blizzardObject;
    Animator animatorSnow;
    Animator animatorFog;
    public enum Meteo { NONE, SNOW, BLIZZARD };
    public Meteo weather;
    private Meteo weatherPreviousState;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {
        animatorSnow = blizzardObject.GetComponent<Animator>();
        animatorFog = blizzardObject.transform.GetChild(0).GetComponent<Animator>();
        speed = 1;
        weatherPreviousState = Meteo.NONE;

    }

    private void OnTriggerEnter(Collider other)
    {
        {
            Debug.Log("Entre dans trigger");
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Le tag est player");
                animatorSnow.speed = speed;
                animatorFog.speed = speed;
                if (weather == Meteo.NONE)
                {
                    if (weatherPreviousState == Meteo.BLIZZARD)
                    {
                        animatorSnow.SetTrigger("BlizzardToSnow");
                        animatorFog.SetTrigger("BlizzardToFog");
                    }
                    Debug.Log("snow to none");
                    animatorSnow.SetTrigger("SnowToNone");
                    animatorFog.SetTrigger("FogToNone");
                    weatherPreviousState = weather;


                }
                if (weather == Meteo.SNOW)
                {
                    Debug.Log("La meteo est snow");
                    animatorSnow.ResetTrigger("SnowToNone");
                    animatorFog.ResetTrigger("FogToNone");
                    if (animatorSnow.GetCurrentAnimatorStateInfo(0).IsName("BaseState"))
                    {
                        Debug.Log("none to snow");
                        animatorSnow.SetTrigger("NoneToSnow");
                        animatorFog.SetTrigger("NoneToFog");
                    }
                    if (animatorSnow.GetCurrentAnimatorStateInfo(0).IsName("AnimSnow"))
                    {
                        Debug.Log("blizzard to snow");
                        animatorSnow.SetTrigger("BlizzardToSnow");
                        animatorFog.SetTrigger("BlizzardToFog");
                    }
                    weatherPreviousState = weather;
                }

                if (weather == Meteo.BLIZZARD)
                {
                    Debug.Log("Meteo is blizzard");
                    if (weatherPreviousState == Meteo.NONE)
                    {
                        animatorSnow.SetTrigger("NoneToSnow");
                        animatorFog.SetTrigger("NoneToFog");
                    }
                    Debug.Log("snow to blizzard");
                    animatorSnow.SetTrigger("SnowToBlizzard");
                    animatorFog.SetTrigger("FogToBlizzard");
                    weatherPreviousState = weather;
                }

            }
        }

    }
}
