using UnityEngine;

[CreateAssetMenu(fileName = "MushroomConfig", menuName = "Centipede/Mushroom Config")]
public class MushroomConfig : ScriptableObject
{
    [Header("Visuals")]
    public Sprite[] damageSprites;
    public Sprite[] poisonedSprites;

    [Header("Spawning Field")]
    public float spawnMinX = -8.2f;
    public float spawnMaxX = 8.2f;
    public float spawnMinY = -2f;
    public float spawnMaxY = 4f;
}