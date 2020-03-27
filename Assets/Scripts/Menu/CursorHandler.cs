using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    public Texture2D cursorTexture;
    CursorMode cursorMode = CursorMode.Auto;

    private static CursorHandler p_instance = null;
    public static CursorHandler Instance { get { return p_instance; } }

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
        Cursor.SetCursor(cursorTexture, Vector2.zero, cursorMode);
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            SetCursorVisibility(false);
        }
        else
        {
            SetCursorVisibility(true);
        }
    }
    public void SetCursorVisibility(bool visibility)
    {
        Cursor.visible = visibility;
    }
}
