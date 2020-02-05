using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public GameObject hips;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ResetPos()
    {
        Debug.Log("PROUT");
        player.transform.position = new Vector3(hips.transform.position.x, player.transform.position.y, hips.transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
