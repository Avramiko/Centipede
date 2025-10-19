using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected EnemyManager enemyManager;
    protected MushroomGridManager gridManager;
    protected bool isActive;

    protected virtual void Update()
    {
        if (isActive && !enemyManager.IsPaused)
        {
            OnUpdate();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isActive)
        {
            return;
        }

        if (collider.CompareTag(Constants.BulletTag) && collider.TryGetComponent(out Bullet bullet) && !bullet.IsReturned)
        {
            HandleBulletCollision(bullet);
        }
        else if (collider.CompareTag(Constants.PlayerTag))
        {
            HandlePlayerCollision();
        }
        else
        {
            ProcessCollisionWithOther(collider);
        }
    }

    public virtual void BaseActivate(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
        isActive = true;
    }

    public virtual void Deactivate()
    {
        isActive = false;
        enemyManager = null;
    }

    protected abstract void OnUpdate();
    protected abstract void HandleBulletCollision(Bullet bullet);
    protected abstract void HandlePlayerCollision();
    protected virtual void ProcessCollisionWithOther(Collider2D collider) {}
}