using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLocationScript : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PointComplete();
        }
    }

    private void PointComplete()
    {
        SimpleTimerScript.Instance.AddSeconds();
        GamePointsCounter.Instance.AddPoints();
        GameCoordinator.Instance.PointCompleted();
        UniversalAudioController.Instance.PlayAudioClip(AudioType.PointTaken);
    }
}
