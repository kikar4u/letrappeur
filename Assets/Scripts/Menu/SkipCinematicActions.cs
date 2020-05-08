using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipCinematicActions : MonoBehaviour
{
    Animator animator;
    bool readyToSkip;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetReadyToSkip()
    {
        CinematicManager.Instance.readyToSkip = true;
    }

    public void SetNotReadyToSkip()
    {
        CinematicManager.Instance.readyToSkip = false;
    }

    private void Update()
    {
        Debug.Log(CinematicManager.Instance.readyToSkip);
        if (Input.GetButtonDown("Fire1") && !CinematicManager.Instance.readyToSkip)
        {
            animator.SetTrigger("FadeIn");
        }
    }
}
