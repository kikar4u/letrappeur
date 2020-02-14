﻿using System.Collections;
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
        layer_Mask = LayerMask.GetMask("InteractiveObjects", "BlockingObjects");
    }

    private void Update()
    {
        Debug.DrawRay(player.transform.position, (player.nextMoveDirection - player.transform.position) * 5, Color.blue);
        Debug.Log("Player transform : " + player.transform.position);
        Debug.Log("Nextmovedir " + player.nextMoveDirection);
        Debug.Log(player.nextMoveDirection - player.transform.position);
        if (Physics.Raycast(player.transform.position, (player.nextMoveDirection - player.transform.position), out hit, 0.5f, layer_Mask))
        {
            Debug.Log("Raycast triggered");
            player.blocked = true;
        }
        else
        {
            player.blocked = false;
        }
    }
    public void interactionAnim()
    {
        //Debug.DrawRay(transform.position, (player.nextMoveDirection - player.transform.position) * hit.distance, Color.yellow);
        if (Physics.Raycast(player.transform.position, (player.nextMoveDirection - player.transform.position), out hit, 1, layer_Mask))
        {
            if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "InteractiveObjects")
            {
                obstacle = hit.collider.gameObject.transform.position;
                Debug.Log("La position de l'obstacle" + obstacle);
                transform.position += player.transform.position + obstacle;
                Debug.Log("La position du joueur + obstacle" + transform.position);
                Debug.Log("action confirmée");
                GetComponent<Player>().trapperAnim.SetAnimState(AnimState.JUMP);

                Debug.DrawRay(player.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                // Debug.Log("hit");
            }
        }
        else
        {
            Debug.DrawRay(player.transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
            //Debug.Log("Did not Hit");
        }
    }
}

