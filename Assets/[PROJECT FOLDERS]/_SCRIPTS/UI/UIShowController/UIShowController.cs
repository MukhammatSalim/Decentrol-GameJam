using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShowController : MonoBehaviour
{
    public static UIShowController Instance;

    [SerializeField] private GameplayUIModule gameplayUIModule;
    [SerializeField] private TimesUpUIModule timesUpUIModule;
    private IShowUIModule activeUIModule;

    private void Awake()
    {
        Instance = this;
    }

    private void OpenUIModule(IShowUIModule uiModule)
    {
        if (activeUIModule != null) activeUIModule.HideUI();
        activeUIModule = uiModule;
        activeUIModule.ShowUI();
    }
    public void ShowUIModule(GameplaySceneUIModules type)
    {
        switch (type)
        {
            case GameplaySceneUIModules.Gameplay:
            {
                OpenUIModule(gameplayUIModule);
            }
                break;
            case GameplaySceneUIModules.TimerEnd:
            {
                OpenUIModule(timesUpUIModule);
            }
                break;
        }
    }
}
