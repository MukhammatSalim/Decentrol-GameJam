using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimesUpUIModule : MonoBehaviour, IShowUIModule
{
    [SerializeField] private GameObject _timesUpUIParent;
    [Header("Statistics")]
    [SerializeField] private Text timeText;
    [SerializeField] private Text totalPointsText;
    [SerializeField] private Text orderstext;
    [SerializeField] private Text rewardText;
    [Header("Progression")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text levelFrom;
    [SerializeField] private Text levelTo;
    public Action<int> OnLevelUp;
    [Header("Utilities")]
    [SerializeField] private GameObject playerCarAudio;
    [SerializeField] private Button MainMenuButton;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        MainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
        retryButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
        MainMenuButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MainMenuButton.onClick.RemoveAllListeners();
        retryButton.onClick.RemoveAllListeners();
        PlayerPrefs.Save();
    }

    public void ShowUI()
    {
        _timesUpUIParent.SetActive(true);
        UniversalAudioController.Instance.StopAllAudio();
        UniversalAudioController.Instance.PlayAudioClip(AudioType.TimesUp);
        GatherStatistics();
        playerCarAudio.SetActive(false);
        GameObject audioFolder = GameObject.Find("All Audio Sources");
        audioFolder.SetActive(false);
        SetupProgression();
        StartCoroutine(GiveExp(PlayerPrefs.GetInt("Points")));
    }

    public void HideUI()
    {
        _timesUpUIParent.SetActive(false);
    }

    private void GatherStatistics()
    {
        timeText.text = SimpleTimerScript.Instance.GetTotalSecondsPlayer().ToString();
        totalPointsText.text = PlayerPrefs.GetInt("Points").ToString();
        orderstext.text = PlayerPrefs.GetInt("OrdersCompleted").ToString();
        rewardText.text = (PlayerPrefs.GetInt("Points")).ToString();
    }

    #region Progression

    private void SetupProgression()
    {
        int currentLevel = PlayerPrefs.GetInt("Level",1);
        float progress = PlayerPrefs.GetFloat("Progress",1);
        
        levelFrom.text = "Lvl " + currentLevel.ToString();
        levelTo.text = "Lvl " + (currentLevel+1).ToString();

        for (int i = 0; i < currentLevel; i++)
        {
            progressSlider.maxValue *= 1.2f;
        }
        
        progressSlider.value = progress;
        
    }

    private IEnumerator GiveExp(float TotalPoints)
    {
        Debug.Log("Giving EXP");
        if (progressSlider.value + TotalPoints >= progressSlider.maxValue)
        {
            Debug.Log("EXP is exceeded");
            while (progressSlider.value + TotalPoints >= progressSlider.maxValue)
            {
                Debug.Log("Giving EXP to FULL");
                float pointToFill = progressSlider.maxValue - progressSlider.value;
                TotalPoints -= pointToFill;

                yield return StartCoroutine(AnimateSlider(progressSlider.maxValue));
            
                OnLevelUp?.Invoke(PlayerPrefs.GetInt("Level",1)+ 1);
                LevelUpManager.Instance.GiveReward();
                PlayerPrefs.SetInt("Level",PlayerPrefs.GetInt("Level",1) + 1);
                UniversalAudioController.Instance.PlayAudioClip(AudioType.LevelUp);
                
            
                 // progressSlider.DOValue(0, 05f);
                 // yield return StartCoroutine(AnimateSlider(0));
                 progressSlider.value = 0;
            }

            yield return StartCoroutine(AnimateSlider(TotalPoints));
            // progressSlider.value += TotalPoints;
            PlayerPrefs.SetFloat("Progress", progressSlider.value);
        }
        else
        {
            Debug.Log("EXP is below max");
            yield return StartCoroutine(AnimateSlider(progressSlider.value + TotalPoints));
            // progressSlider.value = PlayerPrefs.GetFloat("Progress", 1) + TotalPoints;
            PlayerPrefs.SetFloat("Progress", progressSlider.value);
        }
        MainMenuButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
    }

    private IEnumerator AnimateSlider(float valueTo)
    {
        while (progressSlider.value < valueTo)
        {
            yield return null;
            progressSlider.value += 2;
        }
    }
    #endregion
    
}
