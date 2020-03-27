using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    Dictionary<int, string> leDicoDesSettings = new Dictionary<int, string>();
    List<string> settings = new List<string>();
    [SerializeField] TMP_Dropdown graphics;
    int currentSettings;

    [SerializeField] Slider AmbientVolume;
    [SerializeField] Slider Music;
    [SerializeField] Slider SFX;
    [SerializeField] Slider masterMix;
    [SerializeField] AudioMixer mixer;
    [Tooltip("Mettez-y toutes les audiosources du menu")]
    [SerializeField] AudioSource[] sources;
    // Start is called before the first frame update
    private void Awake()
    {



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
         if (masterMix != null)
        {
            float masterValue;
            if (mixer.GetFloat("Master", out masterValue))
            {
                masterMix.value = masterValue;
                Debug.Log("caca prout" + masterValue);
                Debug.Log("PUTAIN DE MERDE COUILLE");
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



       
    }
    private void Start()
    {

       

    }
    private void FadeSounds()
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
    {
        QualitySettings.SetQualityLevel(graphics.value, true);
        Debug.Log("Current : " + QualitySettings.GetQualityLevel());
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
