using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Lives UI")]
    [SerializeField] private Image[] lifeIcons;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    private void OnEnable()
    {
        GameplayEvents.ScoreChanged += OnScoreChanged;
        GameplayEvents.LivesChanged += OnLivesChanged;
        GameplayEvents.GameStateChanged += OnGameStateChanged;
        RefreshAllUI();
    }

    private void OnDisable()
    {
        GameplayEvents.ScoreChanged -= OnScoreChanged;
        GameplayEvents.LivesChanged -= OnLivesChanged;
        GameplayEvents.GameStateChanged -= OnGameStateChanged;
    }

    private void RefreshAllUI()
    {
        GameManager gameManager = GameManager.Instance;
        OnScoreChanged(gameManager.CurrentScore);
        OnLivesChanged(gameManager.Lives);
        OnGameStateChanged(gameManager.CurrentState);
    }

    private void OnScoreChanged(int score)
    {
        scoreText.text = $"Score: {score}";
        highScoreText.text = $"High Score: {GameManager.Instance.HighScore}";
    }

    private void OnLivesChanged(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].enabled = i < lives;
        }
    }

    private void OnGameStateChanged(GameState gameState)
    {
        gameOverPanel.SetActive(gameState == GameState.GameOver);
    }

    public void OnRestartButtonClicked()
    {
        gameOverPanel.SetActive(false);
        GameplayEvents.RaiseRestartRequested();
    }
}