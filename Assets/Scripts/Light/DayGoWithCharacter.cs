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
    [SerializeField] private float dayAtTheBeginning =0 , dayAtTheEnd=90;
    [SerializeField] private float luminosityAtTheBeginning=5, luminosityAtTheEnd=25;
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
                DirectionalLight.transform.eulerAngles = new Vector3(Mathf.Clamp(DirectionalLight.transform.eulerAngles.x+player.movementOffset * ratioLight, dayAtTheBeginning, dayAtTheEnd), DirectionalLight.transform.eulerAngles.y, DirectionalLight.transform.eulerAngles.z);
                Color.RGBToHSV((DirectionalLight.GetComponent<Light>().color),out H, out S, out V);
                S=Mathf.Lerp(luminosityAtTheBeginning/100, luminosityAtTheEnd/100, Mathf.InverseLerp(dayAtTheBeginning,dayAtTheEnd, DirectionalLight.transform.eulerAngles.x));
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
