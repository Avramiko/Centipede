using UnityEngine;
using System.Collections.Generic;

public abstract class SpawnerBase<T> where T : MonoBehaviour
{
    protected readonly GameConfig gameConfig;
    protected readonly BestObjectPool<T> ObjectPool;
    protected readonly List<T> active = new();

    protected float timer;
    protected float nextSpawnDelay;
    protected float intervalMultiplier = 1f;

    protected abstract Vector2 SpawnIntervalRange { get; }

    protected SpawnerBase(GameConfig gameConfig, BestObjectPool<T> ObjectPool)
    {
        this.gameConfig = gameConfig;
        this.ObjectPool = ObjectPool;
    }

    public virtual void Tick(float deltaTime, EnemyManager enemyManager) // advances timer and attempts spawn when due
    {
        timer += deltaTime;

        if (timer >= nextSpawnDelay)
        {
            timer = 0f;
            TrySpawn(enemyManager);
        }
    }

    public virtual void SetIntervalMultiplier(float multiplier)
    {
        intervalMultiplier = multiplier;
        ScheduleNextSpawn();
    }

    public virtual void Reset(float multiplier)
    {
        intervalMultiplier = multiplier;
        ReleaseAll();
        ScheduleNextSpawn();
        timer = 0f;
    }

    public virtual void Release(T instance)
    {
        if (instance != null && active.Remove(instance))
        {
            OnRelease(instance);
            ObjectPool.ReleaseObject(instance);
        }
    }

    public virtual void ReleaseAll()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            Release(active[i]);
        }
    }

    public virtual void ScheduleNextSpawn() // randomizes next delay within range and applies multiplier
    {
        Vector2 intervalRange = SpawnIntervalRange;
        float interval = Random.Range(intervalRange.x, intervalRange.y) * intervalMultiplier;
        nextSpawnDelay = Mathf.Max(0.01f, interval);
    }

    protected virtual bool TrySpawn(EnemyManager enemyManager) 
    {
        if (!CanSpawn())
        {
            return false;
        }

        T instance = ObjectPool.GetObject();
        ActivateInstance(instance, enemyManager);
        active.Add(instance);

        return true;
    }

    protected (Vector2 position, Vector2 direction) GetOffscreenSpawnPoint(Vector2 verticalRange) // picks a side and y within range
    {
        Vector2 direction;
        float spawnBoundX;
        float spawnBoundY = Random.Range(verticalRange.x, verticalRange.y);

        if (Random.value < 0.5f)
        {
            direction = Vector2.right;
            spawnBoundX = gameConfig.leftLimit - 1f; // outside left boundary
        }
        else
        {
            direction = Vector2.left;
            spawnBoundX = gameConfig.rightLimit + 1f; // outside right boundary
        }

        return (new Vector2(spawnBoundX, spawnBoundY), direction);
    }

    protected virtual bool CanSpawn()
    {
        return true;
    }

    protected abstract void OnRelease(T instance);
    protected abstract void ActivateInstance(T instance, EnemyManager enemyManager);
}