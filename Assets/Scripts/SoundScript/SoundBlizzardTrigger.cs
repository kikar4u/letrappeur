using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBlizzardTrigger : MonoBehaviour
{
    [SerializeField] AudioSource[] Audio;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Audio[0].isPlaying)
        {
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<_MGR_SoundDesign>().PlaySound("Ambiance", Audio[0]);
        }
    }
}
