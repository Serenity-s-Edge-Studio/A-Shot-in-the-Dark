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

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {

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
        float multi = Mathf.Pow(GameManager.instance.SelectedDifficulty.ZombieIncreaseMulti, difficultyLevel);
        EnemyManager.instance.MaxEnemies = Mathf.Min(Mathf.RoundToInt(EnemyManager.instance.orginalMaxZombies * multi), GameManager.instance.SelectedDifficulty.MaxZombies);
    }

    private void updateUI()
    {
        ScoreText.text = $"Score: {score}";
        DeathScoreText.text = $"Score: {score}";
    }
}
