using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRaycast : MonoBehaviour
{
    RaycastHit hit;
    LayerMask layer_Mask;
    Vector3 obstacle;
    Vector3 playerPosition;
    // Start is called before the first frame update
    void Start()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        layer_Mask = LayerMask.GetMask("InteractiveObjects");
    }
    public void interactionAnim()
    {
        playerPosition = transform.position;
        //Debug.Log("Position du joueur" + transform.position);
        //Debug.Log("Position locale du joueur" + transform.localPosition);
       // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2, layer_Mask))
        {

            Debug.Log("La position du joueur" + playerPosition);

            obstacle = hit.collider.gameObject.transform.position;

            Debug.Log("La position de l'obstacle" + obstacle);

            if (Input.GetButton("Fire1"))
            {

                transform.position += playerPosition + obstacle;

                Debug.Log("La position du joueur + obstacle" + transform.position);

                Debug.Log("action confirmée");
                // lancer animation ici
            }

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            // Debug.Log("hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            // Debug.Log("Did not Hit");
        }
    }
}

