using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    MaterialPropertyBlock block;
    // Start is called before the first frame update
    void Start()
    {
        // You can re-use this block between calls rather than constructing a new one each time.
        block = new MaterialPropertyBlock();

        // You can look up the property by ID instead of the string to be more efficient.
        block.SetColor("_EmissionColor", Color.black);
        block.SetColor("_BaseColor", Color.black);
        // You can cache a reference to the renderer to avoid searching for it.
        GetComponent<Renderer>().SetPropertyBlock(block);

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            GetComponent<Animation>().Play("CheckpointAnimation");

            AddCheckpoint(collider.gameObject.GetComponent<Player>());
        }
    }

    void AddCheckpoint(Player player)
    {
        player.SaveCurrentPosInfo();
        Debug.Log("New checkpoint position set :" + player.transform.position);
        Destroy(this.GetComponent<Collider>());
    }
}
