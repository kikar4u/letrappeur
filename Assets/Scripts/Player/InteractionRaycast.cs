using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRaycast : MonoBehaviour
{
    RaycastHit hit;
    LayerMask layer_Mask;
    Vector3 obstacle;
    Player player;

    void Start()
    {
        player = GetComponent<Player>();
        // Bit shift the index of the layer (8) to get a bit mask
        layer_Mask = LayerMask.GetMask("InteractiveObjects");
    }
    public void interactionAnim()
    {
        Vector3 playerPosition = new Vector3(transform.position.x, player.GetComponent<Collider>().bounds.center.y, transform.position.z);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        if (Physics.Raycast(playerPosition, transform.TransformDirection(Vector3.forward), out hit, 2, layer_Mask))
        {
            //Debug.Log("La position du joueur" + playerPosition);

            obstacle = hit.collider.gameObject.transform.position;
            Debug.Log("La position de l'obstacle" + obstacle);
            transform.position += playerPosition + obstacle;
            Debug.Log("La position du joueur + obstacle" + transform.position);
            Debug.Log("action confirmée");
            GetComponent<Player>().trapperAnim.SetAnimState(AnimState.JUMP);

            Debug.DrawRay(playerPosition, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            // Debug.Log("hit");
        }
        else
        {
            Debug.DrawRay(playerPosition, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
            //Debug.Log("Did not Hit");
        }
    }
}

