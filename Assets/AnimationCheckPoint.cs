using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCheckPoint : MonoBehaviour
{
    [SerializeField] AudioSource checkpointSound;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void PlayCheckPointSound()
    {
        GameObject.FindGameObjectWithTag("Managers").GetComponent<_MGR_SoundDesign>().PlaySound("CheckpointSound", checkpointSound);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
