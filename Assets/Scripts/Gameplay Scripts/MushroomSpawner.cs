using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSpawner : MonoBehaviour
{
    [SerializeField] private MushroomConfig mushroomConfig;
    [SerializeField] private int mushroomCount = 30;
    private Coroutine activeSpawnProcess;

    private void OnEnable()
    {
        GameplayEvents.GameResetRequested += OnGameResetRequested;
    }

    private void OnDisable()
    {
        GameplayEvents.GameResetRequested -= OnGameResetRequested;

        if (activeSpawnProcess != null)
        {
            StopCoroutine(activeSpawnProcess);
            activeSpawnProcess = null;
        }
    }

    private void GenerateField() // spawns mushrooms randomly across the grid limits
    {
        HashSet<Vector2Int> usedCells = new();
        MushroomGridManager mushroomGrid = MushroomGridManager.Instance;

        float spacingX = mushroomGrid.HorizontalSpacing;
        float spacingY = mushroomGrid.VerticalSpacing;

        int minGridX = Mathf.CeilToInt(mushroomConfig.spawnMinX / spacingX);
        int maxGridX = Mathf.FloorToInt(mushroomConfig.spawnMaxX / spacingX);
        int minGridY = Mathf.CeilToInt(mushroomConfig.spawnMinY / spacingY);
        int maxGridY = Mathf.FloorToInt(mushroomConfig.spawnMaxY / spacingY);

        int attempts = 0;
        int maxAttempts = mushroomCount * 10;

        while (usedCells.Count < mushroomCount && attempts < maxAttempts)
        {
            int gridX = Random.Range(minGridX, maxGridX + 1);
            int gridY = Random.Range(minGridY, maxGridY + 1);

            attempts++;

            Vector2Int key = new(gridX, gridY);

            if (usedCells.Add(key))
            {
                Vector2 worldPosition = new(gridX * spacingX, gridY * spacingY);
                mushroomGrid.SpawnMushroom(worldPosition);
            }
        }
    }

    private void OnGameResetRequested()
    {
        if (isActiveAndEnabled)
        {
            if (activeSpawnProcess != null)
            {
                StopCoroutine(activeSpawnProcess);
            }

            activeSpawnProcess = StartCoroutine(GenerateFieldNextFrame());
        }
    }

    private IEnumerator GenerateFieldNextFrame() // waits before regenerating field
    {
        yield return null;
        GenerateField();
        activeSpawnProcess = null;
    }
}