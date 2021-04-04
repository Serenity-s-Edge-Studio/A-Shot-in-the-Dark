using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private int pointsForHit;
    [SerializeField]
    private int pointsForKill;
    [SerializeField]
    private TextMeshProUGUI ScoreText;
    [SerializeField]
    private TextMeshProUGUI DeathScoreText;
    private int score;
    public static ScoreManager instance;
    public event Action<float> UpdateSpawnRate;

    private void Awake()
    {
        instance = this;
    }

    public void scoreHit()
    {
        score += pointsForHit;
        updateUI();
        adjustDifficulty();
    }

    public void scoreKill()
    {
        score += pointsForKill;
        updateUI();
        adjustDifficulty();
    }

    private void adjustDifficulty()
    {
        int difficultyLevel = score / 500;
        UpdateSpawnRate(1f / (difficultyLevel + 1));
        float zombieAmountMulti = Mathf.Pow(GameManager.instance.SelectedDifficulty.ZombieIncreaseMulti, difficultyLevel);
        float zombieHealthMulti = Mathf.Pow(GameManager.instance.SelectedDifficulty.ZombieHealthIncrease, difficultyLevel);
        Enemy.HealthMulti = zombieHealthMulti;
        EnemyManager.instance.MaxEnemies = Mathf.Min(Mathf.RoundToInt(EnemyManager.instance.orginalMaxZombies * zombieAmountMulti), GameManager.instance.SelectedDifficulty.MaxZombies);
    }

    private void updateUI()
    {
        ScoreText.text = $"Score: {score}";
        DeathScoreText.text = $"Score: {score}";
    }
    public void LoadScene(int scene)
    {
        GameManager.instance.LoadScene(scene);
    }
}
