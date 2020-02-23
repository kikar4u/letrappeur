using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TriggerMoveCamera : MonoBehaviour
{
    [SerializeField] float smoothness;
    Camera cam;
    public Transform targetPosition;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("something enter");
        if (other.gameObject.tag == "Player")
        {
            if (cam.GetComponent<CameraFollowing>().focusedOnPlayer)
            {
                Debug.Log(cam);
                cam.GetComponent<CameraFollowing>().focusedOnPlayer = false;
                StartCoroutine(MoveCamera(targetPosition));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!cam.GetComponent<CameraFollowing>().focusedOnPlayer)
            {
                cam.GetComponent<CameraFollowing>().focusedOnPlayer = true;
                StopCoroutine(nameof(MoveCamera));
            }
        }
    }

    IEnumerator MoveCamera(Transform newPos)
    {
        while (cam.transform.position != newPos.position && !cam.GetComponent<CameraFollowing>().focusedOnPlayer)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, newPos.position, smoothness * Time.deltaTime);
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, newPos.rotation, smoothness * Time.deltaTime);
            yield return null;
        }
    }
}
