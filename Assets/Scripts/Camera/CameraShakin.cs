using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShakin : MonoBehaviour
{
    Transform camTransform;

    private bool continuousShake;
    private bool fadingShake;

    public bool alwaysShake;

    [Range(0f, 1f)]
    public float fadeShakeFrequence;
    [Range(0f, 10f)]
    public float shakeSmoothness;
    Vector3 newPos;


    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    private void Start()
    {
        //Permet le léger shake du menu
        if (alwaysShake)
        {
            StartCoroutine(SmoothShake(shakeSmoothness, fadeShakeFrequence));
        }
    }

    void OnEnable()
    {
        continuousShake = false;
        fadingShake = false;
    }

    private void Update()
    {
        if (continuousShake || fadingShake)
        {
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, newPos, shakeSmoothness);
        }
    }

    //On l'utilise pas :l
    public void Shake(float duration, float intensity, float decreaseFactor)
    {
        continuousShake = false;
        StartCoroutine(ShakeFixed(duration, intensity, decreaseFactor));
    }

    public void Shake(float intensity, Transform targetToFollow)
    {
        continuousShake = true;
        //StartCoroutine(ShakeContinue(intensity));
        StartCoroutine(ShakeFollowingTarget(intensity, targetToFollow));

    }

    public void StartSmoothShake(float intensity, float frequency)
    {
        //continuousShake = true;
        //StartCoroutine(ShakeContinue(intensity));
        StartCoroutine(SmoothShake(intensity, frequency));

    }

    public void SetContinuousShake(bool value)
    {
        continuousShake = value;
    }

    //Permet de faire shaker la caméra ponctuellement, mais on l'utilise pas.
    IEnumerator ShakeFixed(float duration, float intensity, float decreaseFactor)
    {
        while (duration > 0)
        {
            camTransform.localPosition = camTransform.gameObject.GetComponent<CameraFollowing>().GetCameraPositionRelativeToPlayer() + Random.insideUnitSphere * intensity * decreaseFactor;
            duration -= Time.deltaTime * decreaseFactor;
            decreaseFactor -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    //IEnumerator ShakeContinue(float intensity)
    //{
    //    Vector3 initialPos = camTransform.position;
    //    while (GetContinuousShake())
    //    {
    //        newPos = camTransform.gameObject.GetComponent<CameraFollowing>().GetCameraPositionRelativeToPlayer() + Random.insideUnitSphere * intensity;

    //        yield return new WaitForSeconds(fadeShakeFrequence);
    //    }
    //    fadingShake = true;
    //    while (intensity > 0)
    //    {
    //        intensity -= Time.deltaTime;
    //        newPos = camTransform.gameObject.GetComponent<CameraFollowing>().GetCameraPositionRelativeToPlayer() + Random.insideUnitSphere * intensity;
    //        yield return new WaitForSeconds(shakeSmoothness);
    //    }
    //    fadingShake = false;
    //    StopCoroutine(ShakeContinue(intensity));
    //}

    IEnumerator ShakeFollowingTarget(float intensity, Transform target)
    {
        while (GetContinuousShake())
        {
            newPos = target.position + Random.insideUnitSphere * intensity;

            yield return new WaitForSeconds(fadeShakeFrequence);
        }
        fadingShake = true;
        while (intensity > 0)
        {
            intensity -= Time.deltaTime;
            newPos = target.position + Random.insideUnitSphere * intensity;
            yield return new WaitForSeconds(shakeSmoothness);
        }
        fadingShake = false;
        StopCoroutine(ShakeFollowingTarget(intensity, target));
    }

    IEnumerator SmoothShake(float frequency, float intensity)
    {
        Vector3 initialPos = camTransform.position;
        newPos = Random.insideUnitSphere * shakeSmoothness + initialPos;

        while (alwaysShake || continuousShake)
        {
            if (alwaysShake || continuousShake)
            {
                newPos = Random.insideUnitSphere * intensity + initialPos;
                camTransform.DOMove(newPos, frequency).SetEase(Ease.InOutCubic);
                yield return new WaitForSeconds(frequency);
            }
            if (alwaysShake || continuousShake)
            {
                camTransform.DOMove(initialPos, frequency).SetEase(Ease.InOutCubic);
                yield return new WaitForSeconds(frequency);
            }
        }
        Debug.Log("Stoped coroutine smoothshake");
        StopCoroutine(SmoothShake(frequency, intensity));
    }

    public bool GetContinuousShake()
    {
        return continuousShake;
    }
    public void StopContinuousShake()
    {
        continuousShake = false;
        camTransform.DOKill();
    }
}
