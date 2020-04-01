using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    #region Components
    [SerializeField] Transform targetToFollow;
    #endregion

    #region Variables
    [Range(0f, 1f)]
    [Tooltip("Coefficient d'interpolation de déplacement de la caméra")]
    [SerializeField] float cameraSlideSmoothness;
    [Range(0f, 1f)]
    [Tooltip("Coefficient d'interpolation de rotation de la caméra")]
    [SerializeField] float cameraRotationSmoothness;
    Quaternion initialRotation;
    Vector3 offset;
    [HideInInspector] public bool focusedOnPlayer;
    #endregion

    private void Start()
    {
        initialRotation = transform.rotation;
        focusedOnPlayer = true;

        //Prend la distance entre la caméra et la cible, ce vecteur est l'offset entre les deux
        offset = targetToFollow.position - transform.position;
    }
    void Update()
    {
        if (targetToFollow != null && focusedOnPlayer)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Vector3 finalPosition = targetToFollow.position - offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, finalPosition, cameraSlideSmoothness);
        transform.position = smoothPosition;

        if (initialRotation != transform.rotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, cameraRotationSmoothness);
        }
    }

    public Vector3 GetCameraPosition()
    {
        return targetToFollow.localPosition - offset;
    }

    public Vector3 GetCameraToPlayerOffset()
    {
        return offset;
    }

    public Quaternion GetInitialRotation()
    {
        return initialRotation;
    }
}
