using System;
using TMPro;
using UnityEngine;

public class DifficultyAdjuster : MonoBehaviour
{
    [SerializeField]
    private int pointsForHit;
    [SerializeField]
    private int pointsForKill;
    public static DifficultyAdjuster instance;
    public event Action<float> UpdateSpawnRate;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        ScoreManager.instance.OnDifficultyIncrease.AddListener(adjustDifficulty);
    }

    public void scoreHit()
    {
        ScoreManager.instance.AddScore(pointsForHit);
    }

    public void scoreKill()
    {
        ScoreManager.instance.AddScore(pointsForKill);
    }

    private void adjustDifficulty(int difficultyLevel)
    {
        UpdateSpawnRate(1f / (difficultyLevel + 1));
        float zombieAmountMulti = Mathf.Pow(GameManager.instance.SelectedDifficulty.ZombieIncreaseMulti, difficultyLevel);
        float zombieHealthMulti = Mathf.Pow(GameManager.instance.SelectedDifficulty.ZombieHealthIncrease, difficultyLevel);
        Enemy.HealthMulti = zombieHealthMulti;
        EnemyManager.instance.MaxEnemies = Mathf.Min(Mathf.RoundToInt(EnemyManager.instance.orginalMaxZombies * zombieAmountMulti), GameManager.instance.SelectedDifficulty.MaxZombies);
    }
    public void GiveUp()
    {
        ScoreManager.instance.ScoreWindow.SetActive(true);
    }
}
