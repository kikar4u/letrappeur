using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTree : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerAnimation(string name)
    {
        animator.SetTrigger(name);
    }

    public void SetNewLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }
}
