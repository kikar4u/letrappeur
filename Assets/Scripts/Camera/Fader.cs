﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fader : MonoBehaviour
{
    Animator fadeAnimator;
    Player player;

    public delegate void FadeOutDelegate();

    public FadeOutDelegate fadeOutDelegate;

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

    public Animator GetAnimator()
    {
        return fadeAnimator;
    }

    public void FadeIn()
    {
        fadeAnimator.SetTrigger("Fade");
    }

    public void FadeOut()
    {
        fadeAnimator.SetTrigger("FadeOut");
    }

    public void FadeOutActions()
    {
        if (fadeOutDelegate != null)
        {
            fadeOutDelegate();
            fadeOutDelegate = null;
        }
    }
}
