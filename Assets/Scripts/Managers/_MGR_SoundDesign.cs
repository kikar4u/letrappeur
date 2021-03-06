﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

public class _MGR_SoundDesign : MonoBehaviour
{
    private static _MGR_SoundDesign p_instance = null;
    public static _MGR_SoundDesign Instance { get { return p_instance; } }
    public AudioMixer masterMixer;

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
        AudioClip audio = mesSon[Random.Range(0, mesSon.Length)];
        if (!audiosource.isPlaying)
        {
            audiosource.clip = audio;
            audiosource.Play();
            return;
        }
    }

    public void InterruptAndPlaySound(string __nom, AudioSource audiosource)
    {
        AudioClip[] mesSon = p_sons[__nom];
        AudioClip audio = mesSon[Random.Range(0, mesSon.Length)];

        audiosource.clip = audio;
        audiosource.Play();

    }

    //Joue un son spécifique
    public void PlaySpecificSound(AudioClip _clip, AudioSource audiosource)
    {
        if (audiosource.clip != _clip)
        {
            audiosource.clip = _clip;
            audiosource.Play();
        }
    }

    public AudioClip GetSpecificClip(string name)
    {
        return p_sons[name][0];
    }

    public void FadeOutSounds(List<AudioSource> sources, float fadeTime)
    {
        Tweener fadeTween;
        Tweener pauseTween;

        //Pause les sons dans 2s
        StartCoroutine(PauseDelay(sources, fadeTime));

        for (int i = 0; i < sources.Count; i++)
        {
            float initialVolume = sources[i].volume;
            //Tween pour fade à 0 le volume
            fadeTween = sources[i].DOFade(0, fadeTime);
            //Tween pour fade au volume initial 
            pauseTween = sources[i].DOFade(initialVolume, fadeTime);
            pauseTween.SetDelay(fadeTime);
        }
    }

    IEnumerator PauseDelay(List<AudioSource> source, float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < source.Count; i++)
        {
            source[i].Pause();
        }
        StopCoroutine(PauseDelay(source, delay));
    }

    public void FadeInSounds(List<AudioSource> sources, float fadeTime)
    {
        Tweener tween;
        for (int i = 0; i < sources.Count; i++)
        {
            float initialVolume = sources[i].volume;
            sources[i].volume = 0;
            sources[i].UnPause();
            tween = sources[i].DOFade(initialVolume, fadeTime);
        }
    }

    public void ChangeMixerVolume(string mixerName, float endValue, float duration)
    {
        float value = 0f;
        masterMixer.GetFloat(mixerName, out value);

        masterMixer.DOSetFloat(mixerName, value + endValue, duration);
    }

    public void ChangeMixerVolume(string mixerName, float endValue)
    {
        float value = 0f;
        masterMixer.GetFloat(mixerName, out value);

        masterMixer.SetFloat(mixerName, value + endValue);
    }

    public List<AudioSource> GetAllPlayingAudioSources()
    {
        List<AudioSource> sources = new List<AudioSource>();
        for (int i = 0; i < FindObjectsOfType<AudioSource>().Length; i++)
        {
            if (FindObjectsOfType<AudioSource>()[i].gameObject.tag != "MainCamera" && FindObjectsOfType<AudioSource>()[i].isPlaying)
                sources.Add(FindObjectsOfType<AudioSource>()[i]);
        }
        return sources;

    }
    public List<AudioSource> GetAllAudioSources()
    {
        List<AudioSource> sources = new List<AudioSource>();
        for (int i = 0; i < FindObjectsOfType<AudioSource>().Length; i++)
        {
            if (FindObjectsOfType<AudioSource>()[i].gameObject.tag != "MainCamera")
                sources.Add(FindObjectsOfType<AudioSource>()[i]);
        }
        return sources;
    }

    //public void PlayMusic(AudioClip audio, GameObject source, float volume)
    //{

    //    source.AddComponent<AudioSource>();
    //    source.GetComponent<AudioSource>().clip = audio;
    //    source.GetComponent<AudioSource>().volume = volume;
    //    source.GetComponent<AudioSource>().Play();
    //}
}
