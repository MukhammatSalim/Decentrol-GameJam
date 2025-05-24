using System;
using System.Collections;
using System.Collections.Generic;
using InsaneSystems.RoadNavigator;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NavigationTargetController : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private Transform[] targets;

    [SerializeField] private Button randomLocBtn;

    private void Awake()
    {
        randomLocBtn.onClick.AddListener(SetRandomTarget);
    }

    private void OnDestroy()
    {
        randomLocBtn.onClick.RemoveAllListeners();
    }

    public void SetRandomTarget()
    {
        navigator.SetTargetPoint(targets[Random.Range(0, targets.Length)].position);
    }
}
