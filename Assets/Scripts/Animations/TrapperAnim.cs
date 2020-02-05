using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    IDLE,
    WALK,
    JUMP,
    BREATH
}

public class TrapperAnim : MonoBehaviour
{
    Animator animator;
    AnimState currentState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentState = AnimState.IDLE;
    }
    public void SetAnimState(AnimState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case AnimState.IDLE:
                animator.SetBool("Walk", false);
                break;
            case AnimState.WALK:
                animator.SetBool("Walk", true);
                break;
            case AnimState.JUMP:
                animator.SetTrigger("JumpOver");
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
