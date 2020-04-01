using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public GameObject hips;
    public Player player;

    //Réinitialise la position du personnage par rapport à son centre (hips)
    public void ResetPos()
    {
        player.transform.position = new Vector3(hips.transform.position.x, player.transform.position.y, hips.transform.position.z);
    }

    //Joue un son de pas
    public void PlayFootstep()
    {
        RaycastHit hit;
        // faire une détection de si c'est de la pierre ou pas
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + player.playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, player.terrainMask) && hit.transform.tag == "rock")
        {
            _MGR_SoundDesign.Instance.PlaySound("FootStepRock", player.audioSource);
        }
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + player.playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, player.terrainMask) && hit.transform.tag == "Wood")
        {
            _MGR_SoundDesign.Instance.PlaySound("FootStepWood", player.audioSource);
            if (UnityEngine.Random.Range(0, 4) == 1)
            {
                _MGR_SoundDesign.Instance.PlaySound("CrackleWood", player.audioSourceOtherFX);
            }
        }
        else
        {
            _MGR_SoundDesign.Instance.PlaySound("FootStepSnow", player.audioSource);
        }

    }
    public void EquipAxe(AudioClip _clip)
    {
        _MGR_SoundDesign.Instance.PlaySpecificSound(_clip, player.audioSourceOtherFX);
    }
}
