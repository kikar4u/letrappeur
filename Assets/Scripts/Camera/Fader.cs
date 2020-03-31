using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Fader : MonoBehaviour
{
    Animator fadeAnimator;

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

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        //transform.Find("Fader").GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
    private void Start()
    {
        fadeAnimator = GetComponent<Animator>();
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
