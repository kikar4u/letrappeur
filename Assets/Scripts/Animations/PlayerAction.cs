using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public GameObject hips;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void ResetPos()
    {
        player.transform.position = new Vector3(hips.transform.position.x, player.transform.position.y, hips.transform.position.z);
    }

    public void PlayFootstep()
    {
        RaycastHit hit;
        // faire une détection de si c'est de la pierre ou pas
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + player.playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, player.terrainMask) && hit.transform.tag == "rock")
        {
            GameObject.FindGameObjectWithTag("Managers").GetComponent<_MGR_SoundDesign>().
                PlaySound("FootStepRock", player.audioSource);
        }
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + player.playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, player.terrainMask) && hit.transform.tag == "Wood")
        {
            GameObject.FindGameObjectWithTag("Managers").GetComponent<_MGR_SoundDesign>().
                PlaySound("FootStepWood", player.audioSource);
            //Debug.Log("Im in there fuckers");
            if (UnityEngine.Random.Range(0, 4) == 1)
            {
                GameObject.FindGameObjectWithTag("Managers").GetComponent<_MGR_SoundDesign>().
                    PlaySound("CrackleWood", player.audioSourceOtherFX);
            }

        }
        else
        {
            GameObject.FindGameObjectWithTag("Managers").GetComponent<_MGR_SoundDesign>().
                PlaySound("FootStepSnow", player.audioSource);
        }

    }
    public void EquipAxe(AudioClip _clip)
    {

        Debug.Log("I'm here fuckers");
        _MGR_SoundDesign.Instance.
            PlaySpecificSound(_clip, player.audioSourceOtherFX);
    }
}
