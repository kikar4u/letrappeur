using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformationInput : MonoBehaviour
{
    [SerializeField] float forceOffset = 0.1f;
    [SerializeField] float force = 10f;
    Vector3[] playerCollisionPoints;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay, out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;
                point += hit.normal * forceOffset;
                deformer.AddDeformingForce(point, force);
            }
            deformer.PushVertex();
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Test")
        {
            playerCollisionPoints = new Vector3[collision.contactCount];
            MeshDeformer deformer = collision.gameObject.GetComponent<MeshDeformer>();
            for (int i = 0; i < playerCollisionPoints.Length; i++)
            {
                playerCollisionPoints[i] = collision.GetContact(i).point;
                Debug.Log(playerCollisionPoints[i]);
                deformer.AddDeformingForce(playerCollisionPoints[i], force);
            }
            deformer.PushVertex();
        }
    }
}
