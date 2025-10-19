using System.Collections.Generic;
using UnityEngine;

public partial class CentipedeManager
{
    private void OnNewWaveRequested(int difficultyLevel)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.GameOver)
        {
            return;
        }

        currentDifficulty = difficultyLevel;
        UpdateSpeedMultiplier();
        SpawnCentipedeWave();
    }

    private void SpawnCentipedeWave()
    {
        float segmentSpacing = gameConfig.horizontalGridSpacing;
        float spawnStartY = gameConfig.topLimit + centipedeConfig.spawnHeightOffset;
        int segmentsPerChain = ResolveWaveSegmentCount();
        int chainCount = ResolveChainCount();

        for (int i = 0; i < chainCount; i++)
        {
            float randomSpawnX = Random.Range(gameConfig.leftLimit, gameConfig.rightLimit);
            Vector2 spawnPosition = gridManager.SnapToGrid(new Vector2(randomSpawnX, spawnStartY));

            CreateChain(spawnPosition, segmentsPerChain, segmentSpacing);
        }
    }

    private void CreateChain(Vector2 spawnPosition, int segmentsPerChain, float segmentSpacing)
    {
        var segments = new List<SegmentController>();

        for (int j = 0; j < segmentsPerChain; j++)
        {
            SegmentController segment = enemyManager.RentSegment();

            segment.transform.position = new Vector2(spawnPosition.x, spawnPosition.y + j * segmentSpacing);
            segment.Setup(this, chainIndex: activeChains.Count, segmentIndex: j);
            segments.Add(segment);
        }

        var chain = new CentipedeChain(segments);

        chain.horizontalDirection = (Random.value < 0.5f) ? Vector2.right : Vector2.left;
        activeChains.Add(chain);
        UpdateIndexForChain(chain, activeChains.Count - 1);
    }

    private int ResolveWaveSegmentCount() // determine how many segments per chain, size with difficulty
    {
        Vector2Int sizeRange = centipedeConfig.sizeRange;
        int difficultOffset = Mathf.Min(currentDifficulty, sizeRange.y - sizeRange.x);
        int minSegments = sizeRange.x + difficultOffset;

        return Random.Range(minSegments, sizeRange.y + 1);
    }

    private int ResolveChainCount() // Increase the number of chains every 5 difficulty levels
    {
        int extraChains = currentDifficulty / centipedeConfig.DifficultyPerExtraChain;

        return Mathf.Clamp(centipedeConfig.initialChainCount + extraChains, 1, centipedeConfig.MaxChainCount);
    }

    private void ClearChains()
    {
        for (int i = activeChains.Count - 1; i >= 0; i--)
        {
            foreach (var segment in activeChains[i].chainSegments)
            {
                enemyManager.ReturnSegment(segment);
            }
        }

        activeChains.Clear();
    }
}