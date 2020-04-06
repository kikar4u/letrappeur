using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum AnimState
{
    IDLE,
    WALK,
    PASSIVE_WALK,
    BLIZZARD_WALK,
    CLIMB,
    BREATH,
    CHOP
}

public class TrapperAnim : MonoBehaviour
{
    Player player;
    AnimState currentState;

    InteractiveObject currentInteractiveObject;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        currentState = AnimState.IDLE;
    }
    private void Update()
    {
        if (currentState == AnimState.CLIMB && currentInteractiveObject != null)
        {
            player.canMove = false;
            player.WalkFollowingPath(currentInteractiveObject.speedDuringClimb, false);

        }
        else
        {
            if (!player.canMove)
                player.canMove = true;
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
        //Selon le précédent état, on retire le booléen associé dans l'animator
        switch (currentState)
        {
            case AnimState.WALK:
                player.animator.SetBool("Walk", false);
                break;
            case AnimState.BLIZZARD_WALK:
                player.animator.SetBool("BlizzardWalk", false);
                break;
            case AnimState.PASSIVE_WALK:
                player.animator.SetBool("Walk", false);
                break;
            case AnimState.BREATH:
                player.animator.SetBool("Breath", false);
                break;
            case AnimState.CHOP:
                player.animator.SetBool("Chopping", false);
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
                //UpdateAnimSpeed(1f);
                break;
            case AnimState.WALK:
                player.animator.SetBool("Walk", true);
                //UpdateAnimSpeed(1f);
                break;
            case AnimState.BLIZZARD_WALK:
                player.animator.SetBool("BlizzardWalk", true);
                UpdateAnimSpeed(0.8f);
                break;
            case AnimState.PASSIVE_WALK:
                player.animator.SetBool("Walk", true);
                UpdateAnimSpeed(0.5f);
                break;
            case AnimState.CLIMB:
                player.animator.SetTrigger("StartClimb");
                //UpdateAnimSpeed(0.8f);
                break;
            case AnimState.BREATH:
                player.animator.SetBool("Breath", true);
                //UpdateAnimSpeed(1f);
                break;
            case AnimState.CHOP:
                player.animator.SetBool("Chopping", true);
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
            if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("AxeMelee"))
            {
                player.animator.Play("AxeMelee", -1, 0f);
            }
            else
            {
                player.animator.SetTrigger("Chop");
            }
        }
    }

    //Permet de changer la vitesse de l'animation, au besoin.
    public void UpdateAnimSpeed(float speed)
    {
        StartCoroutine(SmoothUpdateAnimSpeed(speed));
    }

    IEnumerator SmoothUpdateAnimSpeed(float endSpeed)
    {
        float currentAnimSpeed = player.animator.speed;

        Tweener tweener = DOTween.To(() => currentAnimSpeed, x => currentAnimSpeed = x, endSpeed, 1f);
        tweener.Play();
        while (tweener.IsPlaying())
        {
            player.animator.speed = currentAnimSpeed;
            yield return null;
        }
        tweener.Kill();
        StopCoroutine(SmoothUpdateAnimSpeed(endSpeed));
    }
}
