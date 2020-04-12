using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager _instance = null;
    public static CinematicManager Instance { get { return _instance; } }

    [SerializeField] VideoPlayer mainCamera;
    [SerializeField] GameObject postProcessVolumesContainer;
    List<AudioSource> pausedAudioSources;

    private void Awake()
    {
        pausedAudioSources = new List<AudioSource>();

        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SetVideoPlayer()
    {
        // on récupère le videoplayer sur la main camera
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoPlayer>();
        postProcessVolumesContainer = GameObject.FindGameObjectWithTag("PostProcessVolumes");

    }

    public void LaunchCinematic(VideoClip video)
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = true;
        //Lance une vidéo de cinématique
        mainCamera.SetTargetAudioSource(0, GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>());
        mainCamera.clip = video;
        mainCamera.Play();
        CursorHandler.Instance.SetCursorVisibility(false);

        pausedAudioSources = _MGR_SoundDesign.Instance.GetAllPlayingAudioSources();

        _MGR_SoundDesign.Instance.FadeOutSounds(pausedAudioSources, 1f);
        postProcessVolumesContainer.SetActive(false);
        StartCoroutine(CheckCinematic(video.length));
    }

    IEnumerator CheckCinematic(double videoClipLength)
    {
        int actualScene = SceneManagers.Instance.GetCurrentSceneIndex();
        while (mainCamera.clip != null)
        {
            if (mainCamera.GetComponent<VideoPlayer>().time >= videoClipLength)
            {
                Fader.Instance.FadeOut();
                mainCamera.clip = null;
                if (GameObject.FindGameObjectWithTag("Player") != null)
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = false;
            }
            yield return null;
        }
        StopCoroutine(CheckCinematic(videoClipLength));

        postProcessVolumesContainer.SetActive(true);

        Debug.Log(postProcessVolumesContainer);
        if (actualScene == SceneManagers.Instance.GetCurrentSceneIndex())
        {
            _MGR_SoundDesign.Instance.FadeInSounds(pausedAudioSources, 3f);

            if (Fader.Instance.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("FadeOut"))
            {
                Fader.Instance.GetAnimator().Play("FadeOut", -1, 0f);
            }
        }

        pausedAudioSources.Clear();

    }

    //public void Populate()
    //{
    //    Debug.Log("Populate cinematic manager");
    //    postProcessVolumesContainer = GameObject.FindGameObjectWithTag("PostProcessVolumes");
    //    mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoPlayer>();
    //}
}
