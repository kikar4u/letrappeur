﻿using System.Collections;
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

    [SerializeField] GameObject breathingCanvas;
    public GameObject breathingPrefab;

    private void Start()
    {
        if (breathingCanvas == null)
            breathingCanvas = GameObject.FindGameObjectWithTag("BreathingCanvas");
    }

    public void CreateBreathingCircles(GameObject breathingSystem)
    {
        Instantiate(breathingSystem, breathingCanvas.transform);
    }

}
