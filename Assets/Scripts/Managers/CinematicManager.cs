﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager _instance = null;
    public static CinematicManager Instance { get { return _instance; } }

    [SerializeField] VideoPlayer mainCamera;
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
        postProcessVolumesContainer = GameObject.FindGameObjectWithTag("PostProcessVolumes");

    }

    public void LaunchCinematic(VideoClip video)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = true;
        //Lance une vidéo de cinématique
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
            yield return null;
        }
        StopCoroutine(CheckCinematic(videoClipLength));
    }

    //public void Populate()
    //{
    //    Debug.Log("Populate cinematic manager");
    //    postProcessVolumesContainer = GameObject.FindGameObjectWithTag("PostProcessVolumes");
    //    mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoPlayer>();
    //}
}
