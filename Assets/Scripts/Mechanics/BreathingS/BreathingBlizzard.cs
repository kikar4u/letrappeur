using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingBlizzard : BreathingSystem
{
    protected override bool CheckCircleInBounds()
    {
        if (breathingCirclesData.outerMarginCollider.bounds.Contains(new Vector3(breathingCirclesData.playerBreathCollider.bounds.max.x, breathingCirclesData.playerBreathCollider.bounds.center.y, breathingCirclesData.playerBreathCollider.bounds.max.z))
        && !breathingCirclesData.innerMarginCollider.bounds.Contains(new Vector3(breathingCirclesData.playerBreathCollider.bounds.max.x, breathingCirclesData.playerBreathCollider.bounds.center.y, breathingCirclesData.playerBreathCollider.bounds.max.z)))
        {
            if (canWalkDuringBreathing)
            {
                if (player.trapperAnim.GetCurrentState() != AnimState.BLIZZARD_WALK)
                {
                    player.trapperAnim.SetAnimState(AnimState.BLIZZARD_WALK);
                }
                player.WalkFollowingPath(walkSpeedDuringBreathing, false);
                player.trapperAnim.UpdateAnimSpeed(0.8f);
            }

            return true;
        }
        else
        {
            if (canWalkDuringBreathing)
            {
                player.trapperAnim.UpdateAnimSpeed(0f);
            }
            return false;
        }
    }
}
