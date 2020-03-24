using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTuto : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetTrigger("Show");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetTrigger("Hide");
        }
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
