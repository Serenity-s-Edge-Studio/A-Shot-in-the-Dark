using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        GameManager.LoadScene(index);
    }
    public void scoreHit()
    {
        score += pointsForHit;
        updateUI();
    }
    public void scoreKill()
    {
        score += pointsForKill;
        updateUI();
    }
    private void updateUI()
    {
        ScoreText.text = $"Score: {score}";
        DeathScoreText.text = $"Score: {score}";
    }
}
