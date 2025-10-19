using UnityEngine;

public class LadybugController : EnemyControllerBase
{
    private LadybugConfig ladybugConfig;
    private Rect gnomeArea;
    private Vector2Int lastSpawnKey;
    private float moveSpeed;
    private int minimumSpawnRow;

    public void Activate(EnemyManager enemyManager, LadybugConfig ladybugSettings, Vector2 spawnPosition, Rect gnomeArea)
    {
        base.BaseActivate(enemyManager);
        ladybugConfig = ladybugSettings;
        gridManager = MushroomGridManager.Instance;
        this.gnomeArea = gnomeArea;

        transform.position = new Vector3(spawnPosition.x, spawnPosition.y, transform.position.z);
        moveSpeed = ladybugConfig.speed;
        lastSpawnKey = new Vector2Int(int.MinValue, int.MinValue);
        minimumSpawnRow = gridManager.ToGridKey(new Vector2(gnomeArea.xMin, gnomeArea.yMin)).y + 2;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        gnomeArea = Rect.zero;
        lastSpawnKey = new Vector2Int(int.MinValue, int.MinValue);
        moveSpeed = 0f;
        minimumSpawnRow = 0;
    }

    protected override void OnUpdate()
    {
        transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
        TrySpawnMushroom();

        if (transform.position.y <= gnomeArea.yMin -1)
        {
            Release();
        }
    }

    protected override void HandleBulletCollision(Bullet bullet)
    {
        bullet.ReturnToPool();
        GameplayEvents.RaiseEnemyDestroyed(enemyManager.MainConfig.ladybugPoints);
        GameplayEvents.RaiseLadybugDestroyed();
        Release();
    }

    protected override void HandlePlayerCollision()
    {
        GameplayEvents.RaisePlayerHit();
    }

    private void Release()
    {
        if (isActive)
        {
            isActive = false;
            enemyManager.ReleaseLadybug(this);
        }
    }

    private void TrySpawnMushroom() // attempts to drop a mushroom based on chance
    {
        Vector2 snappedGrid = gridManager.SnapToGrid(transform.position);
        Vector2Int key = gridManager.ToGridKey(snappedGrid);

        if (!CanSpawn(key))
        {
            return;
        }

        lastSpawnKey = key;
        float boostLineY = ladybugConfig.mushroomBoostLineY;
        float spawnChance = (snappedGrid.y <= boostLineY) ? ladybugConfig.mushroomBoostChance : ladybugConfig.mushroomBaseChance;

        if (Random.value <= spawnChance)
        {
            gridManager.SpawnMushroom(snappedGrid);
        }
    }

    private bool CanSpawn(Vector2Int key) // checks if cell is valid for spawning
    {
        return !(key == lastSpawnKey || key.y < minimumSpawnRow ||
            gridManager.HasMushroom(key) || gridManager.HasNearbyMushroom(key));
    }
}