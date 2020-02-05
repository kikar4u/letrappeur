using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _MGR_SoundDesign : MonoBehaviour
{
    private static _MGR_SoundDesign p_instance = null;
    public static _MGR_SoundDesign Instance { get { return p_instance; } }

    [System.Serializable]
    public class Son
    {
        public string nom;
        public AudioClip[] arr_sons;
    }

    // tous les sons à utiliser dans le jeu
    // seront initialisés à la création du manager
    public Son[] sons;
    // tous les audio source prêts à jouer un son
    // plusieurs peuvent être nécessaires car plusieurs sons simultanés possible (e.g. musique+son FX)
    //private List<AudioSource> p_listAudioSource;
    // un dictionnaire pour stocker et accéder aux son du jeu depuis leur nom
    private Dictionary<string, AudioClip[]> p_sons;
    // initialisation du manager
    void Awake()
    {
        // ===>> SingletonMAnager
        //p_listAudioSource = new List<AudioSource>();
        //AudioSource source = gameObject.AddComponent<AudioSource>();
        //p_listAudioSource.Add(source);
        //Check if instance already exists
        if (p_instance == null)
            //if not, set instance to this
            p_instance = this;
        //If instance already exists and it's not this:
        else if (p_instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        // DontDestroyOnLoad(gameObject);   par nécessaire ici car déja fait par script __DDOL sur l'objet _EGO_app qui recueille tous les mgr
        p_sons = new Dictionary<string, AudioClip[]>();
        foreach (Son _son in sons)
            p_sons.Add(_son.nom, _son.arr_sons);
    }
    // jouer un son du jeu
    // vérifier que le son existe
    // trouver un lecteur libre (audioSource) ou en ajouter un s'ils sont tous en lecture
    // jouer le son sur le lecteur libre (avec le délai fourni)
    public void PlaySound(string __nom, AudioSource audiosource)
    {
        AudioClip[] mesSon = p_sons[__nom];
        AudioClip audio=mesSon[Random.Range(0, mesSon.Length)];
                if (!audiosource.isPlaying)
                {
                    audiosource.clip = audio;
                    audiosource.Play();
                    return;
                }
    }
    
    //public void PlayMusic(AudioClip audio, GameObject source, float volume)
    //{

    //    source.AddComponent<AudioSource>();
    //    source.GetComponent<AudioSource>().clip = audio;
    //    source.GetComponent<AudioSource>().volume = volume;
    //    source.GetComponent<AudioSource>().Play();
    //}
}
