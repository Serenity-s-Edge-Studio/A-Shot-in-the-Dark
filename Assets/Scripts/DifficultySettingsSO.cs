using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Difficulty Settings", menuName = "Difficulty")]
public class DifficultySettingsSO : ScriptableObject
{
    [SerializeField]
    private int MinVisibility, MaxVisiblity;
    [SerializeField]
    private AnimationCurve VisibilityToTimeRatio;

    public int MaxZombies;
    [Range(1, 10)]
    public float ZombieIncreaseMulti;
    [Range(1, 2)]
    public float ZombieHealthIncrease;
    public int StartingMaxZombies;
    public float ScoreMultiplier = 1;

    public float ComputeVisibility(float time)
    {
        return MinVisibility + VisibilityToTimeRatio.Evaluate(time) * MaxVisiblity;
    }
}
