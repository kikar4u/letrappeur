using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffortPlay : MonoBehaviour
{
    [SerializeField] AudioSource audiosource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void playEffort()
    {
        _MGR_SoundDesign.Instance.PlaySound("Effort", audiosource);
    }
}
