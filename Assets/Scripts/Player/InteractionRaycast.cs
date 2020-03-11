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
        layer_Mask = LayerMask.GetMask("InteractiveObjects", "BlockingObjects");
    }

    private void Update()
    {
        //Debug.DrawRay(player.transform.position, (player.nextMoveDirection - player.transform.position) * 5, Color.blue);

        if (player.trapperAnim.GetCurrentState() != AnimState.CLIMB)
        {
            if (Physics.Raycast(player.transform.position, (player.nextMoveDirection - player.transform.position), out hit, 0.5f, layer_Mask))
            {
                player.blocked = true;
            }
            else
            {
                player.blocked = false;
            }
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
                GetComponent<Player>().trapperAnim.SetAnimState(AnimState.CLIMB);
                player.trapperAnim.SetCurrentInteractiveObject(hit.collider.gameObject.GetComponent<InteractiveObject>());
                Debug.DrawRay(player.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            }
        }
        else
        {
            Debug.DrawRay(player.transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
        }
    }
}

