using UnityEngine;

public class ScorpionSpawner : SpawnerBase<ScorpionController>
{
    private readonly ScorpionConfig scorpionConfig;
    public ScorpionSpawner(GameConfig gameConfig, ScorpionConfig scorpionConfig, BestObjectPool<ScorpionController> Objectpool) : base(gameConfig, Objectpool)
    {
        this.scorpionConfig = scorpionConfig;
    }

    protected override Vector2 SpawnIntervalRange => scorpionConfig.spawnIntervalRange;

    protected override void ActivateInstance(ScorpionController scorpion, EnemyManager enemyManager)
    {
        (var position, var direction) = GetOffscreenSpawnPoint(scorpionConfig.spawnYRange);
        scorpion.Activate(enemyManager, scorpionConfig, position, direction);
    }

    protected override void OnRelease(ScorpionController scorpion)
    {
        scorpion.Deactivate();
    }
}