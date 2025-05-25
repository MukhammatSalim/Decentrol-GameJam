using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePointsCounter : MonoBehaviour
{
    public static GamePointsCounter Instance;

    [SerializeField] private Text PointsText;
    private void Awake()
    {
        Instance = this;
    }

    public void AddPoints()
    {
        PlayerPrefs.SetInt("Points",PlayerPrefs.GetInt("Points",0) + GameDataSOProvider.Instance.gameDataSo.PointsReward);
        PointsText.text = "Points: " + PlayerPrefs.GetInt("Points", 0);
        
        PlayerPrefs.SetInt("OrdersCompleted",PlayerPrefs.GetInt("OrdersCompleted", 0) +1);
    }

    private void OnDestroy()
    {
        PlayerPrefs.DeleteKey("Points");
        PlayerPrefs.DeleteKey("OrdersCompleted");
    }
}
