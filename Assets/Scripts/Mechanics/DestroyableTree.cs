using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTree : MonoBehaviour
{
    Animator animator;
    [SerializeField] AudioClip audioclip;
    [SerializeField] AudioSource audiosource;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerAnimation(string name)
    {
        animator.SetTrigger(name);
        GameObject.FindGameObjectWithTag("Managers").GetComponent<_MGR_SoundDesign>().PlaySpecificSound(audioclip, audiosource);
    }

    public void SetNewLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }
}
