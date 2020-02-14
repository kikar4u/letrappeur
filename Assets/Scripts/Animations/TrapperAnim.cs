using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    IDLE,
    WALK,
    JUMP,
    ESCALADE,
    BREATH
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
        if (currentState == AnimState.ESCALADE && currentInteractiveObject != null)
        {
            Debug.Log("FJSQDIOFJIOQSDJFIQSDJFIODS");
            GetComponent<Player>().WalkFollowingPath(currentInteractiveObject.speedDuringClimb);
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
            case AnimState.ESCALADE:
                animator.SetTrigger("ESCALADE");
                
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
