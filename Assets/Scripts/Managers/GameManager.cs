using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Core References")]
    [SerializeField] private GnomeController gnomeController;

    [Header("Gameplay Settings")]
    [SerializeField] private float respawnDelay = 1.25f;
    [SerializeField] private float invulnerabilityDuration = 2.5f;
    [SerializeField] private float blinkInterval = 0.15f;

    private const int StartingLivesValue = 3;
    private int currentScore;
    private int highScore;
    private int lives;
    private int difficultyLevel;
    private bool isRespawning;
    private GameState currentState = GameState.Ready;
    private Coroutine respawnRoutine;

    public int CurrentScore => currentScore;
    public int HighScore => highScore;
    public int Lives => lives;
    public GameState CurrentState => currentState;

    protected void Awake()
    {
        highScore = PlayerPrefs.GetInt(Constants.HighScoreKey, 0);
    }

    private void OnEnable()
    {
        GameplayEvents.EnemyDestroyed += OnEnemyDestroyed;
        GameplayEvents.MushroomHit += OnMushroomHit;
        GameplayEvents.MushroomDestroyed += OnMushroomDestroyed;
        GameplayEvents.PlayerHit += OnPlayerHit;
        GameplayEvents.CentipedeCleared += OnCentipedeCleared;
        GameplayEvents.RestartRequested += OnRestartRequested;
    }

    private void OnDisable()
    {
        GameplayEvents.EnemyDestroyed -= OnEnemyDestroyed;
        GameplayEvents.MushroomHit -= OnMushroomHit;
        GameplayEvents.MushroomDestroyed -= OnMushroomDestroyed;
        GameplayEvents.PlayerHit -= OnPlayerHit;
        GameplayEvents.CentipedeCleared -= OnCentipedeCleared;
        GameplayEvents.RestartRequested -= OnRestartRequested;
    }

    private void Start()
    {
        StartNewGame();
    }

    private void StartNewGame() // resets all stats and restarts game
    {
        StopRespawnRoutine();

        lives = StartingLivesValue;
        currentScore = 0;
        difficultyLevel = 0;
        isRespawning = false;

        SetState(GameState.Playing);
        GameplayEvents.RaiseGameResetRequested();
        GameplayEvents.RaiseDifficultyLevelChanged(difficultyLevel);
        GameplayEvents.RaiseNewWaveRequested(difficultyLevel);
        GameplayEvents.RaiseScoreChanged(currentScore);
        GameplayEvents.RaiseLivesChanged(lives);

        gnomeController.ResetToSpawn();
    }

    private void OnRestartRequested()
    {
        StartNewGame();
    }

    private void OnPlayerHit()
    {
        if (currentState == GameState.Playing && !isRespawning)
        {
            lives--;
            GameplayEvents.RaiseLivesChanged(lives);
            GameplayEvents.RaiseLifeLost();

            if (lives <= 0)
            {
                TriggerGameOver();
            }
            else
            {
                BeginRespawn();
            }
        }
    }

    private void OnCentipedeCleared()
    {
        if (currentState != GameState.GameOver)
        {
            difficultyLevel++;
            GameplayEvents.RaiseDifficultyLevelChanged(difficultyLevel);
            GameplayEvents.RaiseNewWaveRequested(difficultyLevel);
        }
    }

    private void AddScore(int points) // adds points and updates high score
    {
        currentScore += points;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(Constants.HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        GameplayEvents.RaiseScoreChanged(currentScore);
    }

    private void BeginRespawn()
    {
        StopRespawnRoutine();
        respawnRoutine = StartCoroutine(RespawnRoutine());
    }

    private void StopRespawnRoutine()
    {
        if (respawnRoutine != null)
        {
            StopCoroutine(respawnRoutine);
            respawnRoutine = null;
        }

        isRespawning = false;
    }

    private void TriggerGameOver()
    {
        StopRespawnRoutine();
        gnomeController.SetControlsEnabled(false);
        gnomeController.SetCollidersEnabled(false);
        SetState(GameState.GameOver);
    }

    private void SetState(GameState newState) // updates game state if changed
    {
        if (currentState != newState)
        {
            currentState = newState;
            GameplayEvents.RaiseGameStateChanged(currentState);
        }
    }

    private IEnumerator RespawnRoutine() // waits before respawning player with invulnerability
    {
        isRespawning = true;
        SetState(GameState.Respawning);
        gnomeController.SetControlsEnabled(false);

        yield return new WaitForSeconds(respawnDelay);

        gnomeController.SetControlsEnabled(true);
        gnomeController.StartInvulnerability(invulnerabilityDuration, blinkInterval);
        isRespawning = false;
        SetState(GameState.Playing);
        respawnRoutine = null;
    }

    private void OnEnemyDestroyed(int points) => AddScore(points);
    private void OnMushroomHit(int points) => AddScore(points);
    private void OnMushroomDestroyed(int points) => AddScore(points);
    public int GetHighScore() => highScore;
}