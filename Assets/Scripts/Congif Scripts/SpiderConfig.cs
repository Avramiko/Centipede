using UnityEngine;

[CreateAssetMenu(fileName = "SpiderConfig", menuName = "Centipede/Spider Config")]
public class SpiderConfig : ScriptableObject
{
    [Header("Spawning")]
    public Vector2 spawnIntervalRange = new Vector2(8f, 16f);

    [Header("Movement")]
    public Vector2 verticalRange = new Vector2(-4.1f, -1.8f);
    public Vector2 horizontalSpeedRange = new Vector2(4f, 7f);
    public Vector2 wavePeriodRange = new Vector2(1.2f, 2.2f);
    [Min(0f)] public float movementAngleRange = 45f;
    [Min(0f)] public float speedMultiplier = 1f;

    [Header("Bounds")]
    public float exitPadding = 1.5f;
}