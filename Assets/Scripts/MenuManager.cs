﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
<<<<<<< HEAD
    Dictionary<int, string> leDicoDesSettings = new Dictionary<int, string>();
    List<string> settings = new List<string>();
    [SerializeField] TMP_Dropdown graphics;
    [SerializeField] TMP_Text label;
    int currentSettings;

    [SerializeField] Slider AmbientVolume;
    [SerializeField] Slider Music;
    [SerializeField] Slider SFX;
    [SerializeField] Slider masterMix;
    [SerializeField] AudioMixer mixer;

    [Tooltip("Mettez tous les sons dans ce tableau pour fade leur volume lors de la cinématique :)")]
    [SerializeField] AudioSource[] sources;
    // Start is called before the first frame update
    private void Awake()
    {

        currentSettings = QualitySettings.GetQualityLevel();

        for (int i = 0; i < QualitySettings.names.Length; i++)
        {

            Debug.Log(QualitySettings.names.Length);
            settings.Add(QualitySettings.names[i]);
            leDicoDesSettings.Add(i, QualitySettings.names[i]);
        }
        if (graphics != null)
        {
            graphics.AddOptions(settings);
        }

    }
    private void Start()
    {
        if (masterMix != null)
        {
            float masterValue;
            if (mixer.GetFloat("Master", out masterValue))
            {
                masterMix.value = masterValue;
                Debug.Log("caca prout" + masterValue);
            }
            float AmbianceValue;
            if (mixer.GetFloat("Ambiance", out AmbianceValue))
            {
                AmbientVolume.value = AmbianceValue;
                Debug.Log("caca prout" + AmbianceValue);
            }
            float SFXValue;
            if (mixer.GetFloat("SFX", out SFXValue))
            {
                SFX.value = SFXValue;
                Debug.Log("caca prout" + SFXValue);
            }
            float musicValue;
            if (mixer.GetFloat("Musique", out musicValue))
            {
                Music.value = musicValue;
                Debug.Log("caca prout" + musicValue);
            }
        }

        Fader.Instance.fadeOutDelegate += FadeOutSounds;
    }

    private void FadeOutSounds()
    {
        _MGR_SoundDesign.Instance.FadeOutSounds(sources, 2f);
    }

    public void changeSlider(Slider slider)
    {
        //Debug.Log(mixer);
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
=======
    // Start is called before the first frame update
    void Start()
>>>>>>> 3bbff2783a5366910e20663662eb353446b85a40
    {
        
    }

<<<<<<< HEAD
=======
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Options()
    {

    }
>>>>>>> 3bbff2783a5366910e20663662eb353446b85a40
    public void ExitGame()
    {
        Application.Quit();
    }
}
