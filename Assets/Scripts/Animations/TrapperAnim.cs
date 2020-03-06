﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    IDLE,
    WALK,
    JUMP,
    CLIMB,
    BREATH,
    CHOP
}

public class TrapperAnim : MonoBehaviour
{
    Animator animator;
    AnimState currentState;

    InteractiveObject currentInteractiveObject;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentState = AnimState.IDLE;
    }
    private void Update()
    {
        if (currentState == AnimState.CLIMB && currentInteractiveObject != null)
        {
            GetComponent<Player>().WalkFollowingPath(currentInteractiveObject.speedDuringClimb);

            GetComponent<Player>().canMove = false;
        }
        else
        {
            if (!GetComponent<Player>().canMove)
                GetComponent<Player>().canMove = true;
        }
    }

    public void SetCurrentInteractiveObject(InteractiveObject _interactiveObject)
    {
        currentInteractiveObject = _interactiveObject;
    }

    public void SetAnimState(AnimState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case AnimState.IDLE:
                if (currentInteractiveObject != null)
                    currentInteractiveObject = null;
                animator.SetBool("Walk", false);
                break;
            case AnimState.WALK:
                animator.SetBool("Walk", true);
                break;
            case AnimState.CLIMB:
                animator.SetTrigger("StartClimb");
                break;
            default:
                break;
        }
    }

    public AnimState GetCurrentState()
    {
        return currentState;
    }
}
