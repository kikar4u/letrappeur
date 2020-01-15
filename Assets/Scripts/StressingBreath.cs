using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressingBreath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("LeftTrigger")>=1)
        {
            Debug.Log("pute");
        }

        if (Input.GetAxis("RightTrigger") >= 1)
        {
            Debug.Log("puta");
        }
    }
}
