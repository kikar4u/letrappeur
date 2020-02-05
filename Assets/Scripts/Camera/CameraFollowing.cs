using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    #region Components
    [SerializeField] Transform targetToFollow;
    #endregion

    #region Variables
    [SerializeField] float cameraSlideSpeed;
    Quaternion initialRotation;
    float xOffset;
    #endregion

    private void Start()
    {
        initialRotation = transform.rotation;

        xOffset = targetToFollow.position.x - transform.position.x;
    }
    void Update()
    {
        if (targetToFollow != null)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Vector3 finalPosition = new Vector3(targetToFollow.position.x - xOffset, transform.position.y, transform.position.z);
        Vector3 smoothPosition = Vector3.Lerp(transform.position, finalPosition, cameraSlideSpeed);
        transform.position = smoothPosition;
    }
}
