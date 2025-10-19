using UnityEngine;

public class SpiderSpawner : SpawnerBase<SpiderController>
{
    private readonly SpiderConfig spiderConfig;

    public SpiderSpawner(GameConfig gameConfig, SpiderConfig spiderConfig, BestObjectPool<SpiderController> Objectpool) : base(gameConfig, Objectpool)
    {
        this.spiderConfig = spiderConfig;
    }

    protected override Vector2 SpawnIntervalRange => spiderConfig.spawnIntervalRange;

    protected override void ActivateInstance(SpiderController spider, EnemyManager enemyManager)
    {
        (var spawnPosition, var spawnDirection) = GetOffscreenSpawnPoint(spiderConfig.verticalRange);
        spider.Activate(enemyManager, spiderConfig, spawnPosition, spawnDirection);
    }

    protected override void OnRelease(SpiderController spider)
    {
        spider.Deactivate();
    }
}