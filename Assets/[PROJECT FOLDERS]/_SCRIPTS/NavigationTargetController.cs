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
    private GameObject lastTarget;

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
        if (lastTarget != null) lastTarget.gameObject.SetActive(false);
        lastTarget = GetRandomTarget();
        lastTarget.gameObject.SetActive(true);
        navigator.SetTargetPoint(lastTarget.transform.position);
    }

    private GameObject GetRandomTarget()
    {
        GameObject nextTarget = null;
        while (nextTarget == null)
        {
            GameObject tempObj = targets[Random.Range(0, targets.Length)].gameObject;
            if (lastTarget != null || lastTarget != tempObj) nextTarget = tempObj;
        }
        return nextTarget;
    }
}
