using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Image rewardImage;
    [SerializeField] private Text rewardText;
    [SerializeField] private Button rewardButton;
    [Header("Ordinary reward")] 
    [SerializeField] private Sprite moneySprite;
    [SerializeField] private Sprite specialSprite;

    private void Awake()
    {
        Instance = this;
        rewardButton.onClick.AddListener(() => {levelUpPanel.SetActive(false);});
        
    }

    private void OnDestroy()
    {
        rewardButton.onClick.RemoveAllListeners();
    }

    public void GiveReward()
    {
        levelUpPanel.SetActive(true);
        if (PlayerPrefs.GetInt("Level",1) == 1)
        {
            // Give special reward
            rewardImage.sprite = specialSprite;
            rewardText.text = "Ice cream Sticker";
            return;
        }
        rewardText.text = "$500";
        rewardImage.sprite = moneySprite;
    }
}
