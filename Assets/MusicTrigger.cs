using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider objet)
    {
        if(objet.gameObject.tag == "Player")
            GameObject.FindGameObjectWithTag("Managers").GetComponent<_MGR_SoundDesign>().PlaySpecificSound(clip, source);
    }
}
