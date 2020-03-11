using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingManager : MonoBehaviour
{
    private static BreathingManager _instance;
    public static BreathingManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    GameObject breathingCanvas;
    public GameObject breathingPrefab;

    BreathingSystem currentBreathingSystem;

    private void Start()
    {
        if (breathingCanvas == null)
            breathingCanvas = GameObject.FindGameObjectWithTag("BreathingCanvas");
    }

    public GameObject CreateBreathingCircles(GameObject breathingSystem)
    {
        return Instantiate(breathingSystem, breathingCanvas.transform);
    }

    public void SetBreathingCanvas()
    {
        if (breathingCanvas == null)
            breathingCanvas = GameObject.FindGameObjectWithTag("BreathingCanvas");
    }

    public void SetCurrentBreathing(BreathingSystem breathingSystem)
    {
        currentBreathingSystem = breathingSystem;
    }

    public BreathingSystem GetCurrentBreathing()
    {
        return currentBreathingSystem;
    }
}
