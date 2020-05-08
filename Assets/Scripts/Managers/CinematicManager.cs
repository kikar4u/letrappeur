using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager _instance = null;
    public static CinematicManager Instance { get { return _instance; } }

    [SerializeField] VideoPlayer mainCamera;
    [SerializeField] GameObject postProcessVolumesContainer;
    [SerializeField] GameObject skipCinematicHUD;
    List<AudioSource> pausedAudioSources;

    bool inCinematic = false;
    bool stopCinematic = false;
    [HideInInspector] public bool readyToSkip = false;

    [HideInInspector] public UnityEvent VideoEnd = new UnityEvent();

    private void Awake()
    {
        pausedAudioSources = new List<AudioSource>();

        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        skipCinematicHUD.SetActive(false);
    }

    public void SetVideoPlayer()
    {
        // on récupère le videoplayer sur la main camera
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoPlayer>();
        postProcessVolumesContainer = GameObject.FindGameObjectWithTag("PostProcessVolumes");

    }
    private void Update()
    {
        if (readyToSkip && inCinematic && !stopCinematic && Input.GetButtonDown("Fire1"))
        {
            Fader.Instance.fadeOutDelegate += StopCinematic;
            Fader.Instance.FadeIn();
        }
    }

    private void StopCinematic()
    {
        stopCinematic = true;
    }

    public void LaunchCinematic(VideoClip video)
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = true;
        //Lance une vidéo de cinématique
        mainCamera.SetTargetAudioSource(0, GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>());
        mainCamera.clip = video;
        inCinematic = true;
        mainCamera.Play();
        skipCinematicHUD.SetActive(true);
        CursorHandler.Instance.SetCursorVisibility(false);

        pausedAudioSources = _MGR_SoundDesign.Instance.GetAllPlayingAudioSources();

        _MGR_SoundDesign.Instance.FadeOutSounds(pausedAudioSources, 1f);
        postProcessVolumesContainer.SetActive(false);
        StartCoroutine(CheckCinematic(video.length));
    }

    IEnumerator CheckCinematic(double videoClipLength)
    {
        int actualScene = SceneManagers.Instance.GetCurrentSceneIndex();
        bool hasFaded = false;
        while (mainCamera.clip != null)
        {
            //Commence à FadeIn un peu plus tôt
            if (mainCamera.GetComponent<VideoPlayer>().time >= videoClipLength - Fader.Instance.GetAnimator().GetCurrentAnimatorStateInfo(0).length && !hasFaded)
            {
                Fader.Instance.FadeIn();
                hasFaded = true;
            }
            if (mainCamera.GetComponent<VideoPlayer>().time >= videoClipLength || stopCinematic)
            {
                mainCamera.clip = null;
                if (GameObject.FindGameObjectWithTag("Player") != null)
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = false;
            }
            yield return null;
        }
        VideoEnd?.Invoke();
        StopCoroutine(CheckCinematic(videoClipLength));
        stopCinematic = false;
        inCinematic = false;
        skipCinematicHUD.SetActive(false);
        postProcessVolumesContainer.SetActive(true);

        if (actualScene == SceneManagers.Instance.GetCurrentSceneIndex())
        {
            _MGR_SoundDesign.Instance.FadeInSounds(pausedAudioSources, 3f);

            //if (Fader.Instance.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("FadeOut"))
            //{
            //    Fader.Instance.GetAnimator().Play("FadeOut", -1, 0f);
            //}
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
