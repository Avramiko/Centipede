using UnityEngine;

[CreateAssetMenu(fileName = "LadybugConfig", menuName = "Centipede/Ladybug Config")]
public class LadybugConfig : ScriptableObject
{
    [Header("Spawning")]
    public Vector2 spawnIntervalRange = new Vector2(14f, 22f);
    public float spawnHeightOffset = 0.5f;

    [Header("Movement")]
    public float speed = 3.5f;

    [Header("Mushroom Spawning Behavior")]
    public float mushroomBaseChance = 0.65f;
    public float mushroomBoostChance = 0.95f;
    public float mushroomBoostLineY = -2f;
    public int spawnMushroomLimit = 6;
}