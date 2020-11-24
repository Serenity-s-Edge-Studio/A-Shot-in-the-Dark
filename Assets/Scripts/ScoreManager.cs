using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadScene(int index)
    {
        GameManager.instance.LoadScene(index);
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
        EnemyManager.instance.spawnRate = EnemyManager.instance.originalSpawnRate / (difficultyLevel + 1);
        EnemyManager.instance.maxEnemies = Mathf.RoundToInt(EnemyManager.instance.orginalMaxZombies * Mathf.Pow(1.2f, difficultyLevel));
    }

    private void updateUI()
    {
        ScoreText.text = $"Score: {score}";
        DeathScoreText.text = $"Score: {score}";
    }
}
