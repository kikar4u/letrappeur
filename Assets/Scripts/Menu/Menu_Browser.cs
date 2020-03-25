using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;

public class Menu_Browser : MonoBehaviour
{
    [SerializeField] Button[] menuButtons;
    [SerializeField] UnityEvent playEvents;
    [SerializeField] Canvas mainCanvas;

    void Start()
    {
        mainCanvas = GetComponent<Canvas>();
        Fader.Instance.fadeOutDelegate += playEvents.Invoke;
        Fader.Instance.fadeOutDelegate += HideCanvas;

        menuButtons[0].Select();
    }

    private void HideCanvas()
    {
        mainCanvas.gameObject.SetActive(false);
    }

}
