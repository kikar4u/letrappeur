﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagers : MonoBehaviour
{
    public string[] arr_SceneName;      // tableau des scenes

    private static SceneManagers p_instance = null;
    public static SceneManagers Instance { get { return p_instance; } }

    AsyncOperation asyncLoadScene;

    void Awake()
    {
        //Check if instance already exists
        if (p_instance == null)
            //if not, set instance to this
            p_instance = this;
        //If instance already exists and it's not this:
        else if (p_instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += PopulateManagers;
    }

    public void PlayGame()
    {
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("GameManager"));
        SceneManager.LoadScene("Level_1");
    }

    //public IEnumerator LoadSceneAsync(int sceneIndex, float timeToWait)
    //{
    //    asyncLoadScene = SceneManager.LoadSceneAsync(sceneIndex);
    //    asyncLoadScene.allowSceneActivation = false;
    //    yield return new WaitForSeconds(timeToWait);
    //    CinematicManager.Instance.VideoEnd.AddListener(AllowScene);
    //    //Fader.Instance.GetAnimator().Play(Fader.Instance.GetAnimator().GetCurrentAnimatorStateInfo(0).fullPathHash, 0, -1);
    //}


    public void LoadSceneAsync(int sceneIndex)
    {
        asyncLoadScene = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoadScene.allowSceneActivation = false;
        CinematicManager.Instance.VideoEnd.AddListener(AllowScene);
    }

    private void AllowScene()
    {
        asyncLoadScene.allowSceneActivation = true;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void LoadScene(string __nom_scene)
    {
        SceneManager.LoadScene(__nom_scene);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public int GetCurrentSceneIndex()
    {
        //Debug.Log("Current scene index : " + SceneManager.GetActiveScene().buildIndex);
        return SceneManager.GetActiveScene().buildIndex;
    }

    public int GetScenesCount()
    {
        return SceneManager.sceneCountInBuildSettings;
    }

    private void PopulateManagers(Scene scene, LoadSceneMode sceneMode)
    {
        //Peuple les différentes variables des managers propres à la scene
        if (CinematicManager.Instance != null)
            CinematicManager.Instance.SetVideoPlayer();

        if (BreathingManager.Instance != null)
            BreathingManager.Instance.SetBreathingCanvas();

        if (PostProcessManager.Instance != null)
            PostProcessManager.Instance.InitializePostProcess();
    }
}
