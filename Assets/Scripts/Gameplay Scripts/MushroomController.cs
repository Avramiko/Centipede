using UnityEngine;

public class MushroomController : MonoBehaviour
{
    [SerializeField] private MushroomConfig mushroomConfig;
    [SerializeField] private Collider2D[] damageColliders;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private const int MushroomHealth = 4;
    private int currentHealth;
    private bool isPoisoned;
    private Vector2Int gridKey;
    private MushroomGridManager gridManager;
    public bool IsPoisoned => isPoisoned;
    public Vector2Int GridKey => gridKey;

    private void OnEnable()
    {
        currentHealth = MushroomHealth;
        isPoisoned = false;
        UpdateVisuals();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Bullet bullet) && !bullet.IsReturned)
        {
            TakeDamage(bullet);
        }
    }

    public void Initialize(MushroomGridManager gridManager, Vector2Int gridKey, Vector2 worldPosition, bool startPoisoned)
    {
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        this.gridManager = gridManager;
        this.gridKey = gridKey;

        UpdateSortingOrder();

        if (startPoisoned)
        {
            SetPoisoned(true);
        }
    }

    public void TakeDamage(Bullet bullet) // decreases health and destroys mushroom if needed
    {
        if (currentHealth <= 0)
        {
            return;
        }

        bullet.ReturnToPool();
        GameplayEvents.RaiseMushroomHit(gridManager.Config.mushroomHitPoints);
        currentHealth--;

        if (currentHealth <= 0)
        {
            GameplayEvents.RaiseMushroomDestroyed(gridManager.Config.mushroomDestroyBonus);
            gridManager.Release(this);
        }
        else
        {
            UpdateVisuals();
        }
    }

    public void SetPoisoned(bool poisonedState)
    {
        if (isPoisoned != poisonedState)
        {
            isPoisoned = poisonedState;

            if (isPoisoned)
            {
                GameplayEvents.RaiseMushroomPoisoned();
            }

            UpdateVisuals();
        }
    }

    private void UpdateVisuals() // updates sprite based on health and poison status
    {
        int healthState = MushroomHealth - currentHealth;
        spriteRenderer.sprite = isPoisoned ? mushroomConfig.poisonedSprites[healthState] : mushroomConfig.damageSprites[healthState];

        for (int i = 0; i < damageColliders.Length; i++)
        {
            damageColliders[i].enabled = i == healthState;
        }
    }

    private void UpdateSortingOrder()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100f);
    }
}