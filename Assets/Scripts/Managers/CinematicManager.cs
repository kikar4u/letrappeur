using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager _instance = null;
    public static CinematicManager Instance { get { return _instance; } }

    VideoPlayer mainCamera;
    [SerializeField] GameObject postProcessVolumesContainer;

    private void Awake()
    {
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

    }

    public void LaunchCinematic(VideoClip video)
    {
        //Lance une vidéo de cinématique
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = true;
        mainCamera.SetTargetAudioSource(0, GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>());
        mainCamera.clip = video;
        mainCamera.Play();
        postProcessVolumesContainer.SetActive(false);
        StartCoroutine(CheckCinematic(video.length));
    }

    IEnumerator CheckCinematic(double videoClipLength)
    {
        while (mainCamera.clip != null)
        {
            if (mainCamera.GetComponent<VideoPlayer>().time >= videoClipLength)
            {
                Fader.Instance.FadeOut();
                mainCamera.clip = null;
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = false;
                postProcessVolumesContainer.SetActive(true);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void Populate()
    {
        postProcessVolumesContainer = GameObject.FindGameObjectWithTag("PostProcessVolumes");
    }
}
