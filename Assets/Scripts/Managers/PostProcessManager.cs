using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

    #region Vigneting
    Vignette vigneting;
    VignetingData vignetingData;
    bool isVigneting;
    #endregion

    private void OnEnable()
    {
        isVigneting = false;
    }
    public void InitializePostProcess()
    {
        postProcessLayer = FindObjectOfType<PostProcessLayer>();
        vignetingVolume = GameObject.FindGameObjectWithTag("Vignetage").GetComponent<PostProcessVolume>();
        vignetingVolume.profile.TryGetSettings<Vignette>(out vigneting);

        vigneting.intensity.value = 0f;
    }

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
        Debug.Log("Stopvigneting");
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
}
