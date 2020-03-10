using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EventsProvider : MonoBehaviour
{
    public void LaunchCinematic(VideoClip clip)
    {
        Debug.Log("Called");
        Fader.Instance.FadeIn();
        RuntimeAnimatorController rac = Fader.Instance.GetAnimator().runtimeAnimatorController;
        float animDuration = 0;
        for (int i = 0; i < rac.animationClips.Length; i++)
        {
            if (rac.animationClips[i].name == "FadeIn")
            {
                animDuration = rac.animationClips[i].length;
            }
        }
        StartCoroutine(WaitForFadeIn(animDuration, clip));
    }

    IEnumerator WaitForFadeIn(float animDuration, VideoClip clip)
    {
        yield return new WaitForSeconds(animDuration);
        CinematicManager.Instance.LaunchCinematic(clip);
        StopCoroutine(WaitForFadeIn(animDuration, clip));
    }
}
