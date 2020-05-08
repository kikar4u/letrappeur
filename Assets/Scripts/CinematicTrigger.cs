using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class CinematicTrigger : MonoBehaviour
{
    [SerializeField] VideoClip vid;
    VideoPlayer mainCamera;
    double length;

    public UnityEvent cinematicEndEvents;
    // Start is called before the first frame update
    void Start()
    {
        // on récupère le videoplayer sur la main camera
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoPlayer>();
        //length = vid.length;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (mainCamera.clip != null)
    //        CheckIsFinished();
    //}
    //void CheckIsFinished()
    //{
    //    if (mainCamera.GetComponent<VideoPlayer>().time >= length)
    //    {
    //        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = false;
    //        mainCamera.clip = null;

    //        cinematicEndEvents.Invoke();
    //        Fader.Instance.FadeOut();
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        // on vérifie que c'est bien le joueur
        if (other.gameObject.tag == "Player")
        {
            Fader.Instance.FadeIn();
            Fader.Instance.fadeOutDelegate += PlayVideo;
            CinematicManager.Instance.VideoEnd.AddListener(PlayEvents);
        }
    }

    void PlayEvents()
    {
        cinematicEndEvents.Invoke();
    }

    void PlayVideo()
    {
        CinematicManager.Instance.LaunchCinematic(vid);
    }
}
