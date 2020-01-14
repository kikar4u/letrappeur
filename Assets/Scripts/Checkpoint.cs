using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            AddCheckpoint(collider.gameObject.GetComponent<Player>());
        }
    }

    void AddCheckpoint(Player player)
    {
        player.checkpointPosition = transform.position;
        Destroy(this);
    }
}
