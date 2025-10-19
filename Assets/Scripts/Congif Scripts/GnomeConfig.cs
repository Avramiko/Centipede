using UnityEngine;

[CreateAssetMenu(fileName = "GnomeConfig", menuName = "Centipede/Gnome Config")]
public class GnomeConfig : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public Rect movementBounds = new Rect(-8.4f, -4.24f, 16.8f, 2.24f);

    [Header("Shooting")]
    public Bullet bulletPrefab;
    public float fireRate = 0.2f;
}