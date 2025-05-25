using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIModule : MonoBehaviour,IShowUIModule
{
    [SerializeField] private GameObject RCCPCanvas;
    [SerializeField] private GameObject[] OtherGameplayUI;
    public void ShowUI()
    {
        // RCCPCanvas.SetActive(true);
        foreach (var obj in OtherGameplayUI) obj.SetActive(true);
    }

    public void HideUI()
    {
        // RCCPCanvas.SetActive(false);
        foreach (var obj in OtherGameplayUI) obj.SetActive(false);
    }
}
