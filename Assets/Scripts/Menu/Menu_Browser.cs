using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Menu_Browser : MonoBehaviour, IPointerClickHandler
{
    GameObject menu;
    [SerializeField] Button[] menuButtons;
    [SerializeField] UnityEvent playEvents;
    [SerializeField] Canvas mainCanvas;

    bool nothingSelected, started;

    void Start()
    {
        started = false;
        menu = GameObject.FindGameObjectWithTag("MenuPrincipal");
        nothingSelected = false;
        mainCanvas = GetComponent<Canvas>();
        Fader.Instance.fadeOutDelegate += playEvents.Invoke;
        Fader.Instance.fadeOutDelegate += HideCanvas;

        menuButtons[0].Select();
        started = true;

        for (int i = 0; i < menuButtons.Length; i++)
        {
            if (menuButtons[i].gameObject.tag == "Quitter")
            {
                menuButtons[i].onClick.RemoveAllListeners();
                menuButtons[i].onClick.AddListener(SceneManagers.Instance.ExitGame);
            }
        }
    }

    //private void CallLoadScene()
    //{
    //    SceneManagers.Instance.LoadScene("MenuAlternatif");
    //}

    private void HideCanvas()
    {
        mainCanvas.gameObject.SetActive(false);
    }

    public void HoverDeselect()
    {
        PlayHighlightSound();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SetNothingSelected(bool noButtonsSelected)
    {
        nothingSelected = noButtonsSelected;
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Update()
    {
        if (nothingSelected)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                menuButtons[menuButtons.Length - 1].Select();
                nothingSelected = false;
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                menuButtons[0].Select();
                nothingSelected = false;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        nothingSelected = true;
    }

    public void ShowOptions()
    {
        MenuManager.Instance.options.SetActive(true);
        menu.SetActive(false);
        nothingSelected = true;
    }

    void PlayHighlightSound()
    {
        _MGR_SoundDesign.Instance.InterruptAndPlaySound("MenuButtonsHover", Camera.main.GetComponent<AudioSource>());
    }

    public void Highlight()
    {
        if (started)
            PlayHighlightSound();
    }
}
