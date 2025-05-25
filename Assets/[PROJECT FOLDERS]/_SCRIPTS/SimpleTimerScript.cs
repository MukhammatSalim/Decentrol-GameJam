using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTimerScript : MonoBehaviour
{
    public static SimpleTimerScript Instance;
    
    [SerializeField] private Text timerText;

    private int secondsToAdd;
    private int _secondsLeft;
    private int totalSecondsPlayer;
    private Coroutine _timerCoroutine;
    public Action OnTimerEnd;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
        secondsToAdd = GameDataSOProvider.Instance.gameDataSo.TimeReward;
    }

    public void SetTimer(int seconds)
    {
        totalSecondsPlayer = seconds;
        _secondsLeft = seconds;
        _timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    public void AddSeconds()
    {
        _secondsLeft += secondsToAdd;
        totalSecondsPlayer += secondsToAdd;
    }

    public void StopTimer()
    {
        StopCoroutine(_timerCoroutine);
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _secondsLeft--;
            timerText.text = _secondsLeft.ToString();
            if (_secondsLeft == 0)
            {
                OnTimerEnd?.Invoke();
                yield break;
            }
        }
    }

    public int GetTotalSecondsPlayer()
    {
        return totalSecondsPlayer;
    }
}
