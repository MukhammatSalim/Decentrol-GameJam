using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCoordinator : MonoBehaviour
{
    public static GameCoordinator Instance;
    
    [SerializeField] private NavigationTargetController navigationController;
    [SerializeField] private SimpleTimerScript timer;
    [SerializeField] private GameObject taskPanel;
    [SerializeField] private Button CloseBtn;
    private void Awake()
    {
        Instance = this;
        CloseBtn.onClick.AddListener(() =>
        {
            StartGame();
            taskPanel.SetActive(false);
        });
    }
    

    private void OnDestroy()
    {
        CloseBtn.onClick.RemoveAllListeners();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        UIShowController.Instance.ShowUIModule(GameplaySceneUIModules.Gameplay);
        timer.SetTimer(GameDataSOProvider.Instance.gameDataSo.StartTimerSeconds);
        navigationController.SetRandomTarget();
        timer.OnTimerEnd += GameOver;
        UniversalAudioController.Instance.PlayAudioClip(AudioType.GameplayBGMusic);
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        UIShowController.Instance.ShowUIModule(GameplaySceneUIModules.TimerEnd);
    }

    public void PointCompleted()
    {
        navigationController.SetRandomTarget();
    }
}
