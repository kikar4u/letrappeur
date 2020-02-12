using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayGoWithCharacter : MonoBehaviour
{
    public GameObject DirectionalLight;

    [SerializeField] private Player player;
    [SerializeField] private float ratioLight =1;
    private Vector3 savedPosition;
    private float H, S, V;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.hasMovementControls && !player.inCinematic && Input.GetAxisRaw("Horizontal") != 0)
        {
            if (player.GetDirection() >= 0 && transform.position.x >= savedPosition.x)
            {
                DirectionalLight.transform.eulerAngles = new Vector3(Mathf.Clamp(DirectionalLight.transform.eulerAngles.x+player.movementOffset * ratioLight, 0, 90), DirectionalLight.transform.eulerAngles.y, DirectionalLight.transform.eulerAngles.z);
                Color.RGBToHSV((DirectionalLight.GetComponent<Light>().color),out H, out S, out V);
                S=Mathf.Lerp(0.05f, 0.25f, Mathf.InverseLerp(0,90, DirectionalLight.transform.eulerAngles.x));
                Debug.Log(S);
                Debug.Log("H" +H+"S"+ S+"V"+ V);
                DirectionalLight.GetComponent<Light>().color = Color.HSVToRGB(H, S, V);
            }
            if (player.GetDirection() <= 0)
            {
                if (savedPosition.x < transform.position.x)
                {
                    savedPosition = transform.position;
                }
            }
           
        }
    }
}
