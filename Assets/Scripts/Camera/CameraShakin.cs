using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakin : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    Transform camTransform;

    // Amplitude of the shake. A larger value shakes the camera harder.
    //public float shakeAmount = 0.7f;
    //public float decreaseFactor = 1.0f;
    private bool continuousShake;
    private bool fadingShake;

    [Range(0f, 1f)]
    public float fadeShakeFrequence;
    [Range(0f, 1f)]
    public float shakeSmoothness;
    Vector3 newPos;


    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        continuousShake = false;
    }

    public void Shake(float duration, float intensity, float decreaseFactor)
    {
        continuousShake = false;
        StartCoroutine(ShakeFixed(duration, intensity, decreaseFactor));
    }

    public void Shake(float intensity)
    {
        continuousShake = true;
        StartCoroutine(ShakeContinue(intensity));
    }

    IEnumerator ShakeFixed(float duration, float intensity, float decreaseFactor)
    {
        while (duration > 0)
        {
            camTransform.localPosition = camTransform.gameObject.GetComponent<CameraFollowing>().GetCameraPosition() + Random.insideUnitSphere * intensity * decreaseFactor;
            duration -= Time.deltaTime * decreaseFactor;
            decreaseFactor -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator ShakeContinue(float intensity)
    {
        while (GetContinuousShake())
        {
            Debug.Log("Dans shakecontinue :" + GetContinuousShake());
            newPos = camTransform.gameObject.GetComponent<CameraFollowing>().GetCameraPosition() + Random.insideUnitSphere * intensity;

            yield return new WaitForSeconds(fadeShakeFrequence);
        }
        fadingShake = true;
        while (intensity > 0)
        {
            intensity -= Time.deltaTime;
            newPos = camTransform.gameObject.GetComponent<CameraFollowing>().GetCameraPosition() + Random.insideUnitSphere * intensity;
            yield return new WaitForSeconds(shakeSmoothness);
        }
        fadingShake = false;
        StopCoroutine(ShakeContinue(intensity));
    }

    private void Update()
    {
        if (continuousShake || fadingShake)
        {
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, newPos, shakeSmoothness);
        }
    }

    public IEnumerator FadeShake(float factor)
    {
        continuousShake = false;
        float lerpV = 0f;
        //while (lerpV <= 1)
        //{
        //    Debug.Log(Vector3.Lerp(camTransform.localPosition, camTransform.gameObject.GetComponent<CameraFollowing>().GetCameraPosition(), lerpV));
        //    lerpV += factor;
        //    camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, camTransform.gameObject.GetComponent<CameraFollowing>().GetCameraPosition(), lerpV);
        yield return new WaitForSeconds(shakeSmoothness);
        //}
    }

    public bool GetContinuousShake()
    {
        return continuousShake;
    }
    public void ToggleContinuousShake()
    {
        continuousShake = !continuousShake;
    }
}
