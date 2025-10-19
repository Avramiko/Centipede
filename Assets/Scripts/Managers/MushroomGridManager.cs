using System.Collections.Generic;
using UnityEngine;

public class MushroomGridManager : Singleton<MushroomGridManager>
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private int mushroomPoolSize = 120;

    private BestObjectPool<MushroomController> mushroomPool;
    private readonly Dictionary<Vector2Int, MushroomController> mushroomsDictionary = new();

    public GameConfig Config => gameConfig;
    public float HorizontalSpacing => gameConfig.horizontalGridSpacing;
    public float VerticalSpacing => gameConfig.verticalGridSpacing;
    public Rect PlayfieldBounds => gameConfig.PlayfieldBounds;

    protected void Awake()
    {
        mushroomPool = new BestObjectPool<MushroomController>(gameConfig.mushroomPrefab, mushroomPoolSize);
    }

    private void OnEnable()
    {
        GameplayEvents.GameResetRequested += OnGameResetRequested;
    }

    private void OnDisable()
    {
        GameplayEvents.GameResetRequested -= OnGameResetRequested;
    }

    public MushroomController SpawnMushroom(Vector2 worldPosition, bool startPoisoned = false) // spawns mushroom at a grid position, avoiding duplicates
    {
        Vector2 snappedGrid = ClampToBounds(SnapToGrid(worldPosition));
        Vector2Int key = ToGridKey(snappedGrid);

        if (mushroomsDictionary.TryGetValue(key, out MushroomController existing))
        {
            if (startPoisoned)
            {
                existing.SetPoisoned(true);
            }

            return existing;
        }

        MushroomController mushroom = mushroomPool.GetObject();
        mushroom.Initialize(this, key, snappedGrid, startPoisoned);
        mushroomsDictionary[key] = mushroom;

        return mushroom;
    }

    public void Release(MushroomController mushroom)
    {
        mushroomsDictionary.Remove(mushroom.GridKey);
        mushroomPool.ReleaseObject(mushroom);
    }

    public bool TryGetPoisonedMushroom(Vector2 worldPosition, out MushroomController mushroom)
    {
        if (TryGetMushroom(worldPosition, out MushroomController foundMushroom) && foundMushroom.IsPoisoned)
        {
            mushroom = foundMushroom;
            return true;
        }

        mushroom = null;
        return false;
    }

    public bool AttemptPoisonMushroom(Vector2 worldPosition)
    {
        if (TryGetMushroom(worldPosition, out MushroomController mushroom))
        {
            mushroom.SetPoisoned(true);
            return true;
        }

        return false;
    }

    public bool HasNearbyMushroom(Vector2Int gridKey) // checks surrounding cells for any mushrooms
    {
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (offsetX == 0 && offsetY == 0)
                {
                    continue;
                }
                if (mushroomsDictionary.ContainsKey(new Vector2Int(gridKey.x + offsetX, gridKey.y + offsetY)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int CountMushroomsInArea(Rect area) // counts mushrooms inside the area
    {
        int counter = 0;

        foreach (var mushroom in mushroomsDictionary.Values)
        {
            if (area.Contains(mushroom.transform.position))
            {
                counter++;
            }
        }

        return counter;
    }

    public void ResetGrid()
    {
        List<MushroomController> activeMushrooms = new List<MushroomController>(mushroomsDictionary.Values);

        foreach (var mushroom in activeMushrooms)
        {
            Release(mushroom);
        }

        mushroomsDictionary.Clear();
    }

    private void OnGameResetRequested()
    {
        ResetGrid();
    }

    public Vector2 SnapToGrid(Vector2 position)
    {
        return GridUtility.SnapToGrid(position, HorizontalSpacing, VerticalSpacing);
    }

    public Vector2 ClampToBounds(Vector2 position)
    {
        return GridUtility.ClampToBounds(position, PlayfieldBounds);
    }

    public Vector2Int ToGridKey(Vector2 position)
    {
        return GridUtility.WorldToGridKey(position, HorizontalSpacing, VerticalSpacing);
    }

    public bool HasMushroom(Vector2 worldPosition)
    {
        return mushroomsDictionary.ContainsKey(ToGridKey(worldPosition));
    }

    public bool HasMushroom(Vector2Int gridKey)
    {
        return mushroomsDictionary.ContainsKey(gridKey);
    }

    public bool TryGetMushroom(Vector2 worldPosition, out MushroomController mushroom)
    {
        return mushroomsDictionary.TryGetValue(ToGridKey(worldPosition), out mushroom);
    }
}