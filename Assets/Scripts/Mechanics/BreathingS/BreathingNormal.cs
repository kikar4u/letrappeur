using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingNormal : BreathingSystem
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
                player.WalkFollowingPath(walkSpeedDuringBreathing);
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
}
