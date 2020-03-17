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
    FloatParameter fpIntensity;
    FloatParameter fpSmoothness;
    #endregion

    public void InitializePostProcess()
    {
        postProcessLayer = FindObjectOfType<PostProcessLayer>();
        vignetingVolume = GameObject.FindGameObjectWithTag("Vignetage").GetComponent<PostProcessVolume>();
        vignetingVolume.profile.TryGetSettings<Vignette>(out vigneting);

        fpIntensity = new FloatParameter();

    }

    public void UpdateVigneting(float intensity, float smoothness)
    {
        fpIntensity.value = intensity;
        Debug.Log(vigneting);
        vigneting.intensity = fpIntensity;
        vigneting.smoothness = fpSmoothness;
    }
}
