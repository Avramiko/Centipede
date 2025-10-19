using UnityEngine;

[CreateAssetMenu(fileName = "CentipedeConfig", menuName = "Centipede/Centipede Config")]
public class CentipedeConfig : ScriptableObject
{
    [Header("Size Settings")]
    public Vector2Int sizeRange = new Vector2Int(10, 16);

    [Header("Movement")]
    public float speed = 6f;

    [Header("Difficulty")]
    [Min(0f)] public float speedGrowthPerWave = 0.15f;
    [Min(1)] public int initialChainCount = 1;

    [Header("Spawning")]
    public Vector2 spawnXRange = new Vector2(-6f, 6f);
    [Min(0f)] public float spawnHeightOffset = 0.5f;

    private const int MaxChainCountValue = 3;
    private const int DifficultyPerExtraChainValue = 5;
    public int MaxChainCount => MaxChainCountValue;
    public int DifficultyPerExtraChain => DifficultyPerExtraChainValue;
}