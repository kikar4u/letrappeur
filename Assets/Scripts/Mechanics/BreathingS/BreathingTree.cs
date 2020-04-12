﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingTree : BreathingSystem
{
    protected override bool CheckCircleInBounds()
    {
        if (breathingCirclesData.outerMarginCollider.bounds.Contains(new Vector3(breathingCirclesData.playerBreathCollider.bounds.max.x, breathingCirclesData.playerBreathCollider.bounds.center.y, breathingCirclesData.playerBreathCollider.bounds.max.z))
        && !breathingCirclesData.innerMarginCollider.bounds.Contains(new Vector3(breathingCirclesData.playerBreathCollider.bounds.max.x, breathingCirclesData.playerBreathCollider.bounds.center.y, breathingCirclesData.playerBreathCollider.bounds.max.z)))
        {
            if (canWalkDuringBreathing)
            {
                if (player.trapperAnim.GetCurrentState() != AnimState.PASSIVE_WALK)
                {
                    player.trapperAnim.SetAnimState(AnimState.PASSIVE_WALK);
                }
                player.WalkFollowingPath(walkSpeedDuringBreathing, false);
            }

            return true;
        }
        else
        {
            if (canWalkDuringBreathing)
            {
                if (player.trapperAnim.GetCurrentState() != AnimState.BREATH)
                {
                    player.trapperAnim.SetAnimState(AnimState.BREATH);
                }
            }
            return false;
        }
    }

    protected override bool CheckPatternSuccess(float successTime)
    {
        if (successTime > (currentBreathing.breathingPattern.animationCurve[currentBreathing.breathingPattern.animationCurve.length - 1].time * currentBreathing.percentSuccessNeeded))
        {
            //on vient de reussir un pattern
            Debug.Log("CHOP");
            player.trapperAnim.Chop();
            return true;
        }
        else
            return false;
    }

    public override void BreathingOver()
    {
        StopAllCoroutines();
        ready = false;
        BreathingManager.Instance.SetCurrentBreathing(null);
        if (triggerBreathing.doCameraShake)
        {
            if (Camera.main.GetComponent<CameraShakin>().GetContinuousShake())
                Camera.main.GetComponent<CameraShakin>().ToggleContinuousShake();
        }

        player.audioSourceBuildRespiration.loop = false;
        AudioClip releaseClip;
        if (haveSucceeded)
        {
            releaseClip = _MGR_SoundDesign.Instance.GetSpecificClip("AfterPanic");
            _MGR_SoundDesign.Instance.PlaySpecificSound(releaseClip, player.audioSourceBuildRespiration);
            player.trapperAnim.SetAnimState(AnimState.IDLE);
        }
        else
        {
            player.audioSourceBuildRespiration.Stop();
            releaseClip = _MGR_SoundDesign.Instance.GetSpecificClip("FailedBreath");

            _MGR_SoundDesign.Instance.PlaySpecificSound(releaseClip, player.audioSource);
            Fader.Instance.fadeOutDelegate += player.Respawn;
            Fader.Instance.FadeIn();
        }
    }

}
