using UnityEngine;

public class ScorpionController : EnemyControllerBase
{
    [SerializeField] private Collider2D poisonCollider;
    private Vector2 moveDirection;
    private float moveSpeed;
    private float poisonCooldown;
    private float lastPoisonTime;

    public void Activate(EnemyManager enemyManager, ScorpionConfig scorpionConfig, Vector2 position, Vector2 direction)
    {
        base.BaseActivate(enemyManager);

        transform.position = position;
        moveDirection = direction;
        moveSpeed = scorpionConfig.speed;
        poisonCooldown = scorpionConfig.poisonCooldown;
        lastPoisonTime = -poisonCooldown;
        spriteRenderer.flipX = moveDirection.x > 0f;
    }

    protected override void OnUpdate()
    {
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        CheckBounds();
    }

    protected override void HandleBulletCollision(Bullet bullet)
    {
        bullet.ReturnToPool();
        GameplayEvents.RaiseEnemyDestroyed(enemyManager.MainConfig.scorpionPoints);
        GameplayEvents.RaiseScorpionDestroyed();
        Release();
    }

    protected override void HandlePlayerCollision()
    {
        GameplayEvents.RaisePlayerHit();
        Release();
    }

    protected override void ProcessCollisionWithOther(Collider2D hitCollider) // handle poisons mushrooms on contact with cooldown
    {
        if (hitCollider.TryGetComponent(out MushroomController mushroom))
        {
            if (hitCollider.IsTouching(poisonCollider))
            {
                if (Time.time >= lastPoisonTime + poisonCooldown)
                {
                    MushroomGridManager.Instance.AttemptPoisonMushroom(mushroom.transform.position);
                    lastPoisonTime = Time.time;
                }
            }
        }
    }

    private void CheckBounds() // despawns when fully outside
    {
        const float padding = 1.5f;

        if ((moveDirection.x < 0f && transform.position.x < enemyManager.MainConfig.leftLimit - padding) ||
            (moveDirection.x > 0f && transform.position.x > enemyManager.MainConfig.rightLimit + padding))
        {
            Release();
        }
    }

    private void Release()
    {
        if (isActive)
        {
            isActive = false;
            enemyManager.ReleaseScorpion(this);
        }
    }
}