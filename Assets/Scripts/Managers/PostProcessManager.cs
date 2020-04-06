using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class PostProcessManager : MonoBehaviour
{
    private static PostProcessManager _instance;
    public static PostProcessManager Instance
    {
        get
        {
            return _instance;
        }
    }
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
    }

    PostProcessLayer postProcessLayer;
    PostProcessVolume vignetingVolume;
    PostProcessVolume bloomVolume;

    #region Vigneting
    Vignette vigneting;
    VignetingData vignetingData;
    bool isVigneting;
    #endregion

    #region Bloom
    Bloom bloom;
    float bloomBaseIntensity;
    //Compliqué à expliqué... Mais ce booléen va permettre d'arrêter les coroutines en cours qui font fade le bloom 
    //pour qu'il n'y en ai toujours qu'une qui influence le bloom
    bool bloomTrick;
    #endregion

    private void OnEnable()
    {
        isVigneting = false;
    }
    public void InitializePostProcess()
    {
        postProcessLayer = FindObjectOfType<PostProcessLayer>();
        vignetingVolume = GameObject.FindGameObjectWithTag("Vignetage").GetComponent<PostProcessVolume>();
        bloomVolume = GameObject.FindGameObjectWithTag("Bloom").GetComponent<PostProcessVolume>();
        vignetingVolume.profile.TryGetSettings<Vignette>(out vigneting);
        bloomVolume.profile.TryGetSettings<Bloom>(out bloom);

        vigneting.intensity.value = 0f;
        bloomBaseIntensity = bloom.intensity.value;
        bloom.intensity.value = 0f;
    }

    #region Vignetting
    public void UpdateVigneting(float intensity, float smoothness)
    {
        vigneting.intensity.value = intensity;
        vigneting.smoothness.value = smoothness;
    }

    IEnumerator VignetingSlowMove()
    {
        float lerpValue = 0;
        float startIntensity = vigneting.intensity.value;
        int inverter = 1;
        while (isVigneting)
        {
            if (lerpValue >= 1f)
            {
                //Reset
                lerpValue = 0f;
                startIntensity = vigneting.intensity.value;
                inverter = -inverter;
            }
            vigneting.intensity.value = Mathf.Lerp(startIntensity, vignetingData.currentAverage + (vignetingData.offset * inverter), lerpValue);
            lerpValue += Time.deltaTime / vignetingData.periodTime;
            yield return null;
        }
        startIntensity = vigneting.intensity.value;
        lerpValue = 0f;
        //Debug.Log("Stopvigneting");
        while (lerpValue < 1f)
        {
            vigneting.intensity.value = Mathf.Lerp(startIntensity, 0f, lerpValue);
            lerpValue += Time.deltaTime / vignetingData.periodTime;
            yield return null;
        }
        StopCoroutine(nameof(VignetingSlowMove));
    }

    public void StartVigneting()
    {
        isVigneting = true;
        StartCoroutine(nameof(VignetingSlowMove));
    }

    public void StopVigneting()
    {
        isVigneting = false;

    }

    IEnumerator VignetingCurrentAverageSlide(float newAverage)
    {
        float oldAverage = vignetingData.currentAverage;
        float lerpValue = 0f;
        while (lerpValue <= 1f)
        {
            vignetingData.currentAverage = Mathf.Lerp(oldAverage, newAverage, lerpValue);
            yield return null;
            lerpValue += Time.deltaTime * vignetingData.periodTime;
        }
        vignetingData.currentAverage = newAverage;
        StopCoroutine(VignetingCurrentAverageSlide(newAverage));
    }

    public void SlideVignetingToIntensity(float newAverage)
    {
        StartCoroutine(VignetingCurrentAverageSlide(newAverage));
    }

    public void SetVignetingData(VignetingData _vignetingData)
    {
        vignetingData = _vignetingData;
    }

    public void SetNewAverageIntensity(float newAverage)
    {
        vignetingData.currentAverage = newAverage;
    }

    public VignetingData GetCurrentVignetingData()
    {
        return vignetingData;
    }
    #endregion

    #region Bloom
    public void SetBloomIntensity(float value, float transitionTime = 0f)
    {
        bloomTrick = !bloomTrick;
        if (transitionTime > 0f)
            StartCoroutine(FadeBloom(transitionTime, value, bloomTrick));
        else
            bloom.intensity.value = value;
    }

    public void SetDefaultBloomIntensity(float transitionTime)
    {
        bloomTrick = !bloomTrick;
        if (transitionTime > 0f)
            StartCoroutine(FadeBloom(transitionTime, bloomBaseIntensity, bloomTrick));
        else
            bloom.intensity.value = bloomBaseIntensity;
    }
    //Fait fade le bloom, si tu comprends pas à quoi sert le _trick, regarde en haut du script à son instantiation 
    IEnumerator FadeBloom(float duration, float endValue, bool _trick)
    {
        bool trick = _trick;
        float lerpValue = 0f;
        float initialValue = bloom.intensity.value;
        while (lerpValue <= 1f && trick == bloomTrick)
        {
            bloom.intensity.value = Mathf.Lerp(initialValue, endValue, lerpValue);
            yield return null;
            lerpValue += Time.deltaTime / duration;
        }
        StopCoroutine(FadeBloom(duration, endValue, _trick));
    }

    #endregion
}
