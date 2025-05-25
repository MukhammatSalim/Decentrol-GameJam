using UnityEngine;
[CreateAssetMenu(fileName = "GameDataSO", menuName = "ScriptableObjects/GameDataSOScript")]
public class GameDataSO : ScriptableObject
{
    public int StartTimerSeconds;
    public int TimeReward;
    public int PointsReward;
}
