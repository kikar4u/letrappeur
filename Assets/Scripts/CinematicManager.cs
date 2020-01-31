using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] VideoClip vid;
    VideoPlayer Camera;
    // Start is called before the first frame update
    void Start()
    {
        // on récupère le videoplayer sur la main camera
        Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        // on vérifie que c'est bien le joueur
        if (other.gameObject.tag == "Player")
        {
            // on assigne le clip
            Camera.clip = vid;
            // et on joue la douce vidéo
            Camera.Play();
        }
    }
}
