using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTree : MonoBehaviour
{
    //[SerializeField] float force = 0f;
    //[SerializeField] Vector3 directionForce;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerAnimation(string name)
    {
        animator.SetTrigger(name);
        gameObject.layer = LayerMask.GetMask("Ground");
    }

    //void Update()
    //{
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        gameObject.GetComponent<Rigidbody>().AddForce(directionForce * force);
    //    }
    //}
}
