using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
}
