using System;

public static class GameplayEvents
{
    public static event Action<Bullet> BulletReleased;
    public static event Action BulletFired;
    public static event Action<int> EnemyDestroyed;
    public static event Action SegmentDestroyed;
    public static event Action<int> MushroomHit;
    public static event Action<int> MushroomDestroyed;
    public static event Action PlayerHit;
    public static event Action LifeLost;
    public static event Action CentipedeCleared;
    public static event Action CentipedeDiveStarted;
    public static event Action LadybugSpawned;
    public static event Action<int> ScoreChanged;
    public static event Action<int> LivesChanged;
    public static event Action<GameState> GameStateChanged;
    public static event Action RestartRequested;
    public static event Action<int> DifficultyLevelChanged;
    public static event Action<int> NewWaveRequested;
    public static event Action GameResetRequested;
    public static event Action MushroomPoisoned;
    public static event Action ScorpionDestroyed;
    public static event Action SpiderDestroyed;
    public static event Action LadybugDestroyed;

    public static void RaiseBulletReleased(Bullet bullet) => BulletReleased?.Invoke(bullet);
    public static void RaiseBulletFired() => BulletFired?.Invoke();
    public static void RaiseEnemyDestroyed(int points) => EnemyDestroyed?.Invoke(points);
    public static void RaiseSegmentDestroyed() => SegmentDestroyed?.Invoke();
    public static void RaiseMushroomHit(int points) => MushroomHit?.Invoke(points);
    public static void RaiseMushroomDestroyed(int points) => MushroomDestroyed?.Invoke(points);
    public static void RaisePlayerHit() => PlayerHit?.Invoke();
    public static void RaiseLifeLost() => LifeLost?.Invoke();
    public static void RaiseCentipedeCleared() => CentipedeCleared?.Invoke();
    public static void RaiseCentipedeDiveStarted() => CentipedeDiveStarted?.Invoke();
    public static void RaiseLadybugSpawned() => LadybugSpawned?.Invoke();
    public static void RaiseScoreChanged(int score) => ScoreChanged?.Invoke(score);
    public static void RaiseLivesChanged(int lives) => LivesChanged?.Invoke(lives);
    public static void RaiseGameStateChanged(GameState state) => GameStateChanged?.Invoke(state);
    public static void RaiseRestartRequested() => RestartRequested?.Invoke();
    public static void RaiseDifficultyLevelChanged(int level) => DifficultyLevelChanged?.Invoke(level);
    public static void RaiseNewWaveRequested(int level) => NewWaveRequested?.Invoke(level);
    public static void RaiseGameResetRequested() => GameResetRequested?.Invoke();
    public static void RaiseMushroomPoisoned() => MushroomPoisoned?.Invoke();
    public static void RaiseScorpionDestroyed() => ScorpionDestroyed?.Invoke();
    public static void RaiseSpiderDestroyed() => SpiderDestroyed?.Invoke();
    public static void RaiseLadybugDestroyed() => LadybugDestroyed?.Invoke();
}