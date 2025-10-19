using UnityEngine;

public class LadybugSpawner : SpawnerBase<LadybugController>
{
    private readonly LadybugConfig ladybugConfig;

    public LadybugSpawner(GameConfig gameConfig, LadybugConfig ladybugConfig, BestObjectPool<LadybugController> ObjectPool) : base(gameConfig, ObjectPool)
    {
        this.ladybugConfig = ladybugConfig;
    }

    protected override Vector2 SpawnIntervalRange => ladybugConfig.spawnIntervalRange;

    protected override bool CanSpawn() // checks if enough space and not too many mushrooms before spawning
    {
        Rect gnomeBounds = GnomeController.Instance.Config.movementBounds;
        int mushroomCount = MushroomGridManager.Instance.CountMushroomsInArea(gnomeBounds);

        return mushroomCount <= ladybugConfig.spawnMushroomLimit;
    }

    protected override void ActivateInstance(LadybugController ladybug, EnemyManager enemyManager)
    {
        Rect gnomeBounds = GnomeController.Instance.Config.movementBounds;
        float spawnX = Random.Range(gnomeBounds.xMin, gnomeBounds.xMax);
        float spawnY = gameConfig.topLimit + ladybugConfig.spawnHeightOffset;

        ladybug.Activate(enemyManager, ladybugConfig, new Vector2(spawnX, spawnY), gnomeBounds);
        GameplayEvents.RaiseLadybugSpawned();
    }

    protected override void OnRelease(LadybugController ladybug)
    {
        ladybug.Deactivate();
    }
}