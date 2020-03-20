﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTree : MonoBehaviour
{
    [SerializeField] float force =0f;
    [SerializeField] Vector3 directionForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.position);
        if (Input.GetButtonDown("Fire1"))
        {
            gameObject.GetComponent<Rigidbody>().AddForce(directionForce * force);
        }
    }
}
