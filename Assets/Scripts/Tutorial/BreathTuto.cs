using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BreathTuto : MonoBehaviour
{
    Animator animator;
    Canvas canvas;
    bool entered;
    [HideInInspector] public bool confirmed;
    [SerializeField] float masterAudioAttenuation;

    private void Start()
    {
        entered = false;
        confirmed = false;
        animator = GetComponent<Animator>();
        canvas = GetComponentInChildren<Canvas>();
        canvas.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !entered)
        {
            entered = true;
            canvas.gameObject.SetActive(true);
            animator.SetTrigger("Show");

            _MGR_SoundDesign.Instance.ChangeMixerVolume("Master", -masterAudioAttenuation, 1f);
        }
    }

    private void Update()
    {
        if (entered && Input.GetMouseButtonDown(0) && !confirmed)
        {
            animator.SetTrigger("Hide");
            Retake();
            confirmed = true;
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Retake()
    {
        Time.timeScale = 1f;
        _MGR_SoundDesign.Instance.ChangeMixerVolume("Master", masterAudioAttenuation, 1f);
    }
}
