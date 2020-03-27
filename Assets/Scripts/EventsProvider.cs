using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EventsProvider : MonoBehaviour
{
    public void LaunchCinematic(VideoClip clip)
    {
        //Fader.Instance.FadeIn();
        RuntimeAnimatorController rac = Fader.Instance.GetAnimator().runtimeAnimatorController;
        float animDuration = 0;

        for (int i = 0; i < rac.animationClips.Length; i++)
        {
            if (rac.animationClips[i].name == "FadeIn")
            {
                animDuration = rac.animationClips[i].length;
            }
        }
        StartCoroutine(WaitForFadeIn(animDuration, clip, false));
    }

    public void LaunchCinematicWithLoadNewScene(VideoClip clip)
    {
        //Fader.Instance.FadeIn();
        RuntimeAnimatorController rac = Fader.Instance.GetAnimator().runtimeAnimatorController;
        float animDuration = 0;

        for (int i = 0; i < rac.animationClips.Length; i++)
        {
            if (rac.animationClips[i].name == "FadeIn")
            {
                animDuration = rac.animationClips[i].length;
            }
        }
        if (SceneManagers.Instance.GetCurrentSceneIndex() < SceneManagers.Instance.GetScenesCount() - 2)
            StartCoroutine(WaitForFadeIn(animDuration, clip, true, SceneManagers.Instance.GetCurrentSceneIndex() + 1));
        else
        {
            StartCoroutine(WaitForFadeIn(animDuration, clip, true));
        }
    }

    IEnumerator WaitForFadeIn(float animDuration, VideoClip clip, bool loadScene, int sceneToLoad = 0)
    {
        //Attend la fin du fade
        yield return new WaitForSeconds(animDuration);

        //Si on change de scene
        if (loadScene)
        {
            //Appelle la coroutine qui load asynchronement
            SceneManagers.Instance.StartCoroutine(SceneManagers.Instance.LoadSceneAsync(sceneToLoad, (float)clip.length));
        }
        CinematicManager.Instance.LaunchCinematic(clip);

    }

}
