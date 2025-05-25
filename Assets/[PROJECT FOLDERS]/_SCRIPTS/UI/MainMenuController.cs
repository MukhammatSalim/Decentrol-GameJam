using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    private void Awake()
    {
        startGameButton.onClick.AddListener(() => {SceneManager.LoadScene(1);});
    }

    private void Start()
    {
        UniversalAudioController.Instance.PlayAudioClip(AudioType.MainMenuBG);
    }

    private void OnDestroy()
    {
        startGameButton.onClick.RemoveAllListeners();
        UniversalAudioController.Instance.StopAllAudio();
    }
}
