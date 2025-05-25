using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameDataSOProvider : MonoBehaviour
{
    public static GameDataSOProvider Instance;
    public GameDataSO gameDataSo;

    private void Awake()
    {
        Instance = this;
    }
    
}
