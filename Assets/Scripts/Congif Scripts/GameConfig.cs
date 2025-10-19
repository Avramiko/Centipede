using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Centipede/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Prefabs & Sprites")]
    public SegmentController segmentPrefab;
    public ScorpionController scorpionPrefab;
    public SpiderController spiderPrefab;
    public LadybugController ladybugPrefab;
    public MushroomController mushroomPrefab;
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite tailSprite;

    [Header("Playfield & Grid")]
    public float bottomLimit = -4.28f;
    public float topLimit = 4.2f;
    public float leftLimit = -8.5f;
    public float rightLimit = 8.5f;
    public float horizontalGridSpacing = 0.4f;
    public float verticalGridSpacing = 0.4f;

    [Header("Difficulty Settings")]
    [Min(0f)] public float enemySpawnIntervalStep = 0.1f;
    [Min(0.1f)] public float minEnemyIntervalMultiplier = 0.3f;

    [Header("Score Settings")]
    public int segmentPoints = 25;
    public int spiderPoints = 500;
    public int ladybugPoints = 200;
    public int scorpionPoints = 1000;
    public int mushroomHitPoints = 1;
    public int mushroomDestroyBonus = 3;

    [Header("Global Collision")]
    public LayerMask obstacleLayer;

    public Rect PlayfieldBounds => new Rect(leftLimit, bottomLimit, rightLimit - leftLimit, topLimit - bottomLimit);
}