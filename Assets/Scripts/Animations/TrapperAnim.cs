using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    IDLE,
    WALK,
    PASSIVE_WALK,
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
            GetComponent<Player>().canMove = false;
            GetComponent<Player>().WalkFollowingPath(currentInteractiveObject.speedDuringClimb);

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
        if (newState == currentState)
        {
            return;
        }
        switch (currentState)
        {
            case AnimState.WALK:
                animator.SetBool("Walk", false);

                break;
            case AnimState.PASSIVE_WALK:
                animator.SetBool("Walk", false);
                break;
            case AnimState.BREATH:
                animator.SetBool("Breath", false);
                break;
            case AnimState.CHOP:
                animator.SetBool("Chopping", false);
                UpdateAnimSpeed(1f);
                break;
            default:
                break;
        }
        currentState = newState;

        switch (currentState)
        {
            case AnimState.IDLE:
                if (currentInteractiveObject != null)
                    currentInteractiveObject = null;
                UpdateAnimSpeed(1f);
                break;
            case AnimState.WALK:
                animator.SetBool("Walk", true);
                UpdateAnimSpeed(1f);
                break;
            case AnimState.PASSIVE_WALK:
                animator.SetBool("Walk", true);
                UpdateAnimSpeed(0.5f);
                break;
            case AnimState.CLIMB:
                animator.SetTrigger("StartClimb");
                UpdateAnimSpeed(0.8f);
                break;
            case AnimState.BREATH:
                animator.SetBool("Breath", true);
                UpdateAnimSpeed(1f);
                break;
            case AnimState.CHOP:
                animator.SetBool("Chopping", true);
                UpdateAnimSpeed(1f);
                break;
            default:
                break;
        }
    }

    public AnimState GetCurrentState()
    {
        return currentState;
    }

    public void Chop()
    {
        if (GetCurrentState() == AnimState.CHOP)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("AxeMelee"))
            {
                animator.Play("AxeMelee", -1, 0f);
            }
            else
            {
                animator.SetTrigger("Chop");
            }
        }
    }

    private void UpdateAnimSpeed(float speed)
    {
        animator.speed = speed;
    }
}
