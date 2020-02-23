using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] VideoClip vid;
    VideoPlayer mainCamera;
    double length;
    // Start is called before the first frame update
    void Start()
    {
        // on récupère le videoplayer sur la main camera
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoPlayer>();
        length = vid.length;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera.clip != null)
            isFinished();
    }
    void isFinished()
    {
        if (mainCamera.GetComponent<VideoPlayer>().time == length)
        {
            Debug.Log("Isterminée");
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inCinematic = false;
            mainCamera.clip = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // on vérifie que c'est bien le joueur
        if (other.gameObject.tag == "Player")
        {
            mainCamera.SetTargetAudioSource(0, GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>());
            other.GetComponent<Player>().inCinematic = true;
            other.GetComponent<Player>().trapperAnim.SetAnimState(AnimState.IDLE);
            // on assigne le clip
            mainCamera.clip = vid;
            // et on joue la douce vidéo
            mainCamera.Play();


        }
    }
}
