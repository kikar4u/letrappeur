using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TriggerMoveCamera : MonoBehaviour
{
    Camera cam;
    public Transform targetPosition;
    [SerializeField] AnimationCurve animationCurve;
    bool shouldMoveToSpot;
    [Header("Shaking")]
    public bool shake;
    [HideInInspector] public float shakeintensity;
    [HideInInspector] public float shakeFrequency;


    private void Start()
    {
        cam = Camera.main;
        shouldMoveToSpot = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            shouldMoveToSpot = true;
            cam.GetComponent<CameraFollowing>().focusedOnPlayer = false;
            StartCoroutine(MoveCameraToSpot(targetPosition));

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            shouldMoveToSpot = false;
            StartCoroutine(nameof(MoveCameraToPlayer));

        }
    }

    IEnumerator MoveCameraToSpot(Transform newPos)
    {
        float timeLimit = animationCurve.keys[animationCurve.length - 1].time;
        float timeCount = 0f;
        Vector3 initialCameraPosition = cam.transform.position;
        Quaternion initialCameraRotation = cam.transform.rotation;
        while (timeCount < timeLimit && shouldMoveToSpot)
        {
            timeCount += Time.deltaTime;
            cam.transform.position = Vector3.Lerp(initialCameraPosition, newPos.position, animationCurve.Evaluate(timeCount));
            cam.transform.rotation = Quaternion.Lerp(initialCameraRotation, newPos.rotation, animationCurve.Evaluate(timeCount));

            yield return null;
        }

        StopCoroutine(nameof(MoveCameraToSpot));
        if (shake && shouldMoveToSpot)
        {
            cam.GetComponent<CameraShakin>().StartSmoothShake(shakeintensity, shakeFrequency);
            cam.GetComponent<CameraShakin>().SetContinuousShake(true);
        }
    }

    IEnumerator MoveCameraToPlayer()
    {
        if (shake)
            cam.GetComponent<CameraShakin>().StopContinuousShake();
        float timeLimit = animationCurve.keys[animationCurve.length - 1].time;
        float timeCount = 0f;
        Vector3 initialCameraPosition = cam.transform.position;
        Quaternion initialCameraRotation = cam.transform.rotation;
        while (timeCount < timeLimit && !shouldMoveToSpot)
        {
            timeCount += Time.deltaTime;
            cam.transform.position = Vector3.Lerp(initialCameraPosition, GameObject.FindGameObjectWithTag("Player").transform.position - cam.GetComponent<CameraFollowing>().GetCameraToPlayerOffset(), animationCurve.Evaluate(timeCount));
            cam.transform.rotation = Quaternion.Lerp(initialCameraRotation, cam.GetComponent<CameraFollowing>().GetInitialRotation(), animationCurve.Evaluate(timeCount));

            yield return null;
        }
        cam.GetComponent<CameraFollowing>().focusedOnPlayer = true;
        StopCoroutine(nameof(MoveCameraToPlayer));

    }
}
