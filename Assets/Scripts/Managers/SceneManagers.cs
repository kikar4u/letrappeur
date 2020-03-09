using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagers : MonoBehaviour
{
    public string[] arr_SceneName;      // tableau des scenes

    private static SceneManagers p_instance = null;
    public static SceneManagers Instance { get { return p_instance; } }

    void Awake()
    {

        // ===>> SingletonMAnager

        //Check if instance already exists
        if (p_instance == null)
            //if not, set instance to this
            p_instance = this;
        //If instance already exists and it's not this:
        else if (p_instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

    }

    //Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //void OnLevelWasLoaded(int level)
    //{
    //    //Réaffecter le breathing canvas dans le breathingManager
    //}

    public void PlayGame()
    {
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("GameManager"));
        SceneManager.LoadScene("Level_1");
    }
    public void Options()
    {

    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string __nom_scene)
    {
        SceneManager.LoadScene(__nom_scene);
    }
}
