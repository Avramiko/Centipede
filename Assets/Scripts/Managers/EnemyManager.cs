using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Main Config")]
    [SerializeField] private GameConfig gameConfig;

    [Header("Enemy Configs")]
    [SerializeField] private ScorpionConfig scorpionConfig;
    [SerializeField] private SpiderConfig spiderConfig;
    [SerializeField] private LadybugConfig ladybugConfig;
    [SerializeField] private int segmentPoolSize = 64;
    [SerializeField] private int scorpionPoolSize = 4;
    [SerializeField] private int spiderPoolSize = 4;
    [SerializeField] private int ladybugPoolSize = 2;

    private BestObjectPool<SegmentController> segmentPool;
    private BestObjectPool<ScorpionController> scorpionPool;
    private BestObjectPool<SpiderController> spiderPool;
    private BestObjectPool<LadybugController> ladybugPool;
    private ScorpionSpawner scorpionSpawner;
    private SpiderSpawner spiderSpawner;
    private LadybugSpawner ladybugSpawner;

    private float spawnIntervalMultiplier = 1f;
    private bool isPaused;

    public GameConfig MainConfig => gameConfig;
    public bool IsPaused => isPaused;

    protected void Awake()
    {
        segmentPool = new BestObjectPool<SegmentController>(gameConfig.segmentPrefab, segmentPoolSize);
        scorpionPool = new BestObjectPool<ScorpionController>(gameConfig.scorpionPrefab, scorpionPoolSize);
        spiderPool = new BestObjectPool<SpiderController>(gameConfig.spiderPrefab, spiderPoolSize);
        ladybugPool = new BestObjectPool<LadybugController>(gameConfig.ladybugPrefab, ladybugPoolSize);

        scorpionSpawner = new ScorpionSpawner(gameConfig, scorpionConfig, scorpionPool);
        spiderSpawner = new SpiderSpawner(gameConfig, spiderConfig, spiderPool);
        ladybugSpawner = new LadybugSpawner(gameConfig, ladybugConfig, ladybugPool);
    }

    private void OnEnable()
    {
        GameplayEvents.GameResetRequested += OnGameResetRequested;
        GameplayEvents.GameStateChanged += OnGameStateChanged;
        GameplayEvents.DifficultyLevelChanged += OnDifficultyLevelChanged;
    }

    private void OnDisable()
    {
        GameplayEvents.GameResetRequested -= OnGameResetRequested;
        GameplayEvents.GameStateChanged -= OnGameStateChanged;
        GameplayEvents.DifficultyLevelChanged -= OnDifficultyLevelChanged;
    }

    private void Start()
    {
        ResetSpawners();
    }

    private void Update()
    {
        if (isPaused)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        scorpionSpawner.Tick(deltaTime, this);
        spiderSpawner.Tick(deltaTime, this);
        ladybugSpawner.Tick(deltaTime, this);
    }

    public void ReturnSegment(SegmentController segment)
    {
        segmentPool.ReleaseObject(segment);
    }

    public void ReleaseScorpion(ScorpionController scorpion)
    {
        scorpionSpawner.Release(scorpion);
    }

    public void ReleaseSpider(SpiderController spider)
    {
        spiderSpawner.Release(spider);
    }

    public void ReleaseLadybug(LadybugController ladybug)
    {
        ladybugSpawner.Release(ladybug);
    }

    private void ResetSpawners()
    {
        scorpionSpawner.Reset(spawnIntervalMultiplier);
        spiderSpawner.Reset(spawnIntervalMultiplier);
        ladybugSpawner.Reset(spawnIntervalMultiplier);
    }

    private void OnGameResetRequested()
    {
        ResetSpawners();
    }

    private void OnGameStateChanged(GameState gameState)
    {
        isPaused = gameState != GameState.Playing;
    }

    private void OnDifficultyLevelChanged(int difficultyLevel) // speed up spawns as difficulty increases
    {
        float step = gameConfig.enemySpawnIntervalStep;
        float minMultiplier = gameConfig.minEnemyIntervalMultiplier;

        spawnIntervalMultiplier = Mathf.Max(minMultiplier, 1f - step * difficultyLevel);
        scorpionSpawner.SetIntervalMultiplier(spawnIntervalMultiplier);
        spiderSpawner.SetIntervalMultiplier(spawnIntervalMultiplier);
        ladybugSpawner.SetIntervalMultiplier(spawnIntervalMultiplier);
    }

    public SegmentController RentSegment() => segmentPool.GetObject();
}