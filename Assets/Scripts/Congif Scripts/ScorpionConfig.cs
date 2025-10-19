using UnityEngine;

[CreateAssetMenu(fileName = "ScorpionConfig", menuName = "Centipede/Scorpion Config")]
public class ScorpionConfig : ScriptableObject
{
    [Header("Spawning")]
    public Vector2 spawnIntervalRange = new Vector2(12f, 20f);
    public Vector2 spawnYRange = new Vector2(3.2f, 4.2f);

    [Header("Movement")]
    public float speed = 7f;

    [Header("Mushroom Interaction")]
    public float poisonCooldown = 0.5f;
}