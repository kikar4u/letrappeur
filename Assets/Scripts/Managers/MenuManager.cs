﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    public static MenuManager Instance
    {
        get
        {
            return _instance;
        }
    }

    #region Options
    public GameObject options;
    Dictionary<int, string> leDicoDesSettings = new Dictionary<int, string>();
    List<string> settings = new List<string>();
    [SerializeField] TMP_Dropdown graphics;
    int currentSettings;
    [SerializeField] Slider AmbientVolume;
    [SerializeField] Slider Music;
    [SerializeField] Slider SFX;
    [SerializeField] Slider masterMix;
    [SerializeField] AudioMixer mixer;

    #endregion
    //[Tooltip("Mettez-y toutes les audiosources du menu")]
    //List<AudioSource> sources;
    // Start is called before the first frame update
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);

        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            settings.Add(QualitySettings.names[i]);
            leDicoDesSettings.Add(i, QualitySettings.names[i]);
        }
        if (graphics != null)
        {
            graphics.AddOptions(settings);
        }
        if (masterMix != null)
        {
            float masterValue;
            if (mixer.GetFloat("Master", out masterValue))
            {
                masterMix.value = masterValue;
            }
            float AmbianceValue;
            if (mixer.GetFloat("Ambiance", out AmbianceValue))
            {
                AmbientVolume.value = AmbianceValue;
            }
            float SFXValue;
            if (mixer.GetFloat("SFX", out SFXValue))
            {
                SFX.value = SFXValue;
            }
            float musicValue;
            if (mixer.GetFloat("Musique", out musicValue))
            {
                Music.value = musicValue;
            }
        }
    }

    private void OnEnable()
    {
        //sources = new List<AudioSource>();
        //for (int i = 0; i < FindObjectsOfType<AudioSource>().Length; i++)
        //{
        //    if (FindObjectsOfType<AudioSource>()[i].gameObject.tag != "MainCamera")
        //        sources.Add(FindObjectsOfType<AudioSource>()[i]);
        //}
    }

    private void Start()
    {
        //Fader.Instance.fadeOutDelegate += FadeSounds;

        graphics.value = QualitySettings.GetQualityLevel();
    }
    //private void FadeSounds()
    //{
    //    _MGR_SoundDesign.Instance.FadeOutSounds(sources, 2f);
    //}

    public void changeSlider(Slider slider)
    {
        string name = slider.name;
        switch (name)
        {
            case "Master":
                mixer.SetFloat("Master", slider.value);//
                break;
            case "AmbientVolume":
                mixer.SetFloat("Ambiance", slider.value);
                break;
            case "SFX":
                mixer.SetFloat("SFX", slider.value);//
                break;
            case "AmbianceMusique":
                mixer.SetFloat("Musique", slider.value);
                break;
            default:
                break;
        }
    }
    public void ChangeGraphicSettings()
    {
        QualitySettings.SetQualityLevel(graphics.value, true);
        Debug.Log("Current : " + QualitySettings.GetQualityLevel());
    }

    // Update is called once per frame
    void Update()
    {
        ////Si on est pas dans le menu principal
        //if (Input.GetKeyDown(KeyCode.Escape) && SceneManagers.Instance.GetCurrentSceneIndex() != 0)
        //{
        //    //Si le joueur est pas en cinématique
        //    Player player = FindObjectOfType<Player>();
        //    if (player != null)
        //    {
        //        if (!player.inCinematic)
        //        {
        //            ShowOptions(true);
        //        }
        //    }
        //}
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowOptions(bool shouldPauseGame)
    {
        options.SetActive(true);
        if (shouldPauseGame)
        {
            _MGR_SoundDesign.Instance.ChangeMixerVolume("Master", -15f);
            CursorHandler.Instance.SetCursorVisibility(true);
            Time.timeScale = 0;
        }
    }

    public void HideOptions()
    {
        options.SetActive(false);

        if (SceneManagers.Instance.GetCurrentSceneIndex() == 0)
        {
            GameObject.FindGameObjectWithTag("MenuPrincipalCanvas").transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            _MGR_SoundDesign.Instance.ChangeMixerVolume("Master", 15f);
            CursorHandler.Instance.SetCursorVisibility(false);
            Time.timeScale = 1;
        }
    }

}
