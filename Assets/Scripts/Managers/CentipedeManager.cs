using System.Collections.Generic;
using UnityEngine;

public partial class CentipedeManager : Singleton<CentipedeManager>
{
    [SerializeField] private CentipedeConfig centipedeConfig;
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private MushroomGridManager gridManager;
    [SerializeField] private EnemyManager enemyManager;

    private readonly List<CentipedeChain> activeChains = new();
    private bool isPaused;
    private int currentDifficulty;
    private float speedMultiplier = 1f;
    private float movementTimer;

    private void OnEnable()
    {
        GameplayEvents.NewWaveRequested += OnNewWaveRequested;
        GameplayEvents.GameResetRequested += OnGameResetRequested;
        GameplayEvents.GameStateChanged += OnGameStateChanged;
        GameplayEvents.DifficultyLevelChanged += OnDifficultyChanged;
    }

    private void OnDisable()
    {
        GameplayEvents.NewWaveRequested -= OnNewWaveRequested;
        GameplayEvents.GameResetRequested -= OnGameResetRequested;
        GameplayEvents.GameStateChanged -= OnGameStateChanged;
        GameplayEvents.DifficultyLevelChanged -= OnDifficultyChanged;
    }

    private void Update()
    {
        if (isPaused || activeChains.Count == 0)
        {
            return;
        }

        UpdateMovement(Time.deltaTime);
    }

    private void UpdateMovement(float deltaTime) // advances path in grid-steps and then applies to segments
    {
        movementTimer += deltaTime;
        float currentMoveSpeed = GetSegmentMoveSpeed();

        if (currentMoveSpeed <= 0)
        {
            return;
        }

        float timePerStep = gameConfig.horizontalGridSpacing / currentMoveSpeed;

        if (movementTimer >= timePerStep)
        {
            movementTimer = 0f;

            for (int i = activeChains.Count - 1; i >= 0; i--)
            {
                if (activeChains[i].chainSegments.Count > 0)
                {
                    Vector2 nextHeadPosition = CalculateNextHeadPosition(activeChains[i]);
                    activeChains[i].AdvancePath(nextHeadPosition);
                }
            }
        }

        foreach (var chain in activeChains)
        {
            if (chain.chainSegments.Count > 0)
            {
                bool isEntering = chain.HeadPosition.y > gameConfig.topLimit;
                chain.RefreshAppearance(gameConfig, isEntering);
                chain.ApplyPathToSegments();
            }
        }
    }

    private Vector2 CalculateNextHeadPosition(CentipedeChain chain) // decides vertical entry, dive, or horizontal step
    {
        if (chain.HeadPosition.y > gameConfig.topLimit)
        {
            return CalculateVerticalStep(chain.HeadPosition, Vector2.down);
        }

        if (chain.isDiving)
        {
            return HandleDivingState(chain);
        }

        return HandleHorizontalMovement(chain);
    }

    private Vector2 HandleDivingState(CentipedeChain chain)  // continues dive; flips to ascend at bottom
    {
        Vector2 nextDiveStep = CalculateVerticalStep(chain.HeadPosition, Vector2.down);

        if (nextDiveStep.y <= gameConfig.bottomLimit || nextDiveStep == chain.HeadPosition)
        {
            chain.isDiving = false;
            chain.verticalDirection = Vector2.up;
        }

        return nextDiveStep;
    }

    private Vector2 HandleHorizontalMovement(CentipedeChain chain)
    {
        Vector2 nextHorizontalStep = chain.HeadPosition + chain.horizontalDirection * gameConfig.horizontalGridSpacing;

        if (gridManager.TryGetPoisonedMushroom(nextHorizontalStep, out _))
        {
            chain.isDiving = true;
            GameplayEvents.RaiseCentipedeDiveStarted();
            return CalculateVerticalStep(chain.HeadPosition, Vector2.down);
        }

        if (IsBlocked(nextHorizontalStep))
        {
            return HandleBlockedPath(chain);
        }

        return gridManager.SnapToGrid(nextHorizontalStep);
    }

    private Vector2 HandleBlockedPath(CentipedeChain chain) // bounce horizontally and step vertically
    {
        chain.horizontalDirection *= -1;
        Vector2 nextVerticalStep = CalculateVerticalStep(chain.HeadPosition, chain.verticalDirection);

        if (nextVerticalStep == chain.HeadPosition)
        {
            chain.verticalDirection *= -1;
            return CalculateVerticalStep(chain.HeadPosition, chain.verticalDirection);
        }

        if (nextVerticalStep.y <= gameConfig.bottomLimit)
        {
            chain.verticalDirection = Vector2.up;
        }
        else if (nextVerticalStep.y >= gameConfig.topLimit)
        {
            chain.verticalDirection = Vector2.down;
        }

        return nextVerticalStep;
    }

    private Vector2 CalculateVerticalStep(Vector2 currentPosition, Vector2 direction)
    {
        float targetY = currentPosition.y + direction.y * gameConfig.verticalGridSpacing;
        targetY = Mathf.Clamp(targetY, gameConfig.bottomLimit, gameConfig.topLimit);

        return gridManager.SnapToGrid(new Vector2(currentPosition.x, targetY));
    }

    private bool IsBlocked(Vector2 position)
    {
        return (position.x < gameConfig.leftLimit || position.x > gameConfig.rightLimit) || gridManager.HasMushroom(position);
    }

    public float GetSegmentMoveSpeed()
    {
        return isPaused ? 0f : centipedeConfig.speed * speedMultiplier;
    }

    public void HandleSegmentHit(SegmentController hitSegment, Bullet bullet) // removes segment, spawns mushroom, and splits chain
    {
        if (bullet.IsReturned)
        {
            return;
        }

        int chainIndex = hitSegment.GetChainIndex();

        if (chainIndex < 0 || chainIndex >= activeChains.Count)
        {
            return;
        }

        bullet.ReturnToPool();

        CentipedeChain chain = activeChains[chainIndex];
        int segmentIndex = hitSegment.GetSegmentIndex();

        TriggerHitEffects(hitSegment.transform.position);
        enemyManager.ReturnSegment(hitSegment);
        chain.chainSegments.RemoveAt(segmentIndex);
        ProcessChainSplit(chain, chainIndex, segmentIndex);
    }

    private void TriggerHitEffects(Vector2 position)
    {
        gridManager.SpawnMushroom(position);
        GameplayEvents.RaiseSegmentDestroyed();
        GameplayEvents.RaiseEnemyDestroyed(gameConfig.segmentPoints);
    }

    private void ProcessChainSplit(CentipedeChain originalChain, int originalChainIndex, int splitIndex)
    {
        CentipedeChain newChain = originalChain.Split(splitIndex);
        UpdateExistingChain(originalChain, originalChainIndex);
        RegisterNewChain(newChain);
    }

    private void UpdateExistingChain(CentipedeChain chain, int chainIndex)
    {
        if (chain.chainSegments.Count > 0)
        {
            UpdateIndexForChain(chain, chainIndex);
            bool isEntering = chain.HeadPosition.y > gameConfig.topLimit;
            chain.RefreshAppearance(gameConfig, isEntering);
        }
        else
        {
            RemoveChain(chainIndex);
        }
    }

    private void RegisterNewChain(CentipedeChain newChain) // add split chain and refresh
    {
        if (newChain != null && newChain.chainSegments.Count > 0)
        {
            activeChains.Add(newChain);
            UpdateIndexForChain(newChain, activeChains.Count - 1);
            bool isNewChainEntering = newChain.HeadPosition.y > gameConfig.topLimit;
            newChain.RefreshAppearance(gameConfig, isNewChainEntering);
        }
    }

    private void RemoveChain(int index)
    {
        activeChains.RemoveAt(index);

        for (int i = index; i < activeChains.Count; i++)
        {
            UpdateIndexForChain(activeChains[i], i);
        }

        if (activeChains.Count == 0)
        {
            GameplayEvents.RaiseCentipedeCleared();
        }
    }

    private void UpdateIndexForChain(CentipedeChain chain, int chainIndex)
    {
        for (int i = 0; i < chain.chainSegments.Count; i++)
        {
            chain.chainSegments[i].UpdateChainAndSegmentIndex(chainIndex, i);
        }
    }

    private void OnGameResetRequested()
    {
        isPaused = false;
        currentDifficulty = 0;
        UpdateSpeedMultiplier();
        ClearChains();
    }

    private void OnGameStateChanged(GameState state) // pause/unpause based on state
    {
        isPaused = (state != GameState.Playing);
    }

    private void OnDifficultyChanged(int difficultLevel)
    {
        currentDifficulty = difficultLevel;
        UpdateSpeedMultiplier();
    }

    private void UpdateSpeedMultiplier()
    {
        speedMultiplier = 1f + centipedeConfig.speedGrowthPerWave * currentDifficulty;
    }
}