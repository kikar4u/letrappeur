using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ce script achève totalement le Single responsabilities du S des principes SOLID
public class FogTrigger : MonoBehaviour
{
    [Range(0, 0.1f)]
    [SerializeField] float finalIntensity;
    [SerializeField] float transitionTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CallFogUpdate();
            Destroy(gameObject);
        }
    }
    public void CallFogUpdate()
    {
        PostProcessManager.Instance.UpdateFogIntensity(finalIntensity, transitionTime);
    }
}
