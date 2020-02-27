using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fader : MonoBehaviour
{
    Animator fadeAnimator;
    Player player;

    public delegate void RespawnDelegate();

    public RespawnDelegate respawnDelegate;

    private static Fader _instance;
    public static Fader Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        fadeAnimator = GameObject.FindGameObjectWithTag("Fader").GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void FadeIn()
    {
        fadeAnimator.SetTrigger("Fade");
    }

    public void RespawnActions()
    {
        if (respawnDelegate != null)
        {
            respawnDelegate();
            Debug.Log(respawnDelegate);
        }
    }

}
