using UnityEngine;

public class SpiderController : EnemyControllerBase
{
    private const float AccelerateMultiplier = 6f;
    private SpiderConfig spiderConfig;
    private int horizontalSign = 1;
    private float baseSpeed;
    private Vector2 currentVelocity;
    private Vector2 desiredVelocity;
    private float directionTimer;
    private float directionDuration;
    private float minY;
    private float maxY;

    public void Activate(EnemyManager enemyManager, SpiderConfig spiderConfig, Vector2 position, Vector2 horizontalVector)
    {
        base.BaseActivate(enemyManager);
        this.spiderConfig = spiderConfig;
        gridManager = MushroomGridManager.Instance;
        horizontalSign = horizontalVector.x >= 0f ? 1 : -1;

        ConfigureMovementBoundaries();
        ConfigureSpeed();
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        PickNewMovementDirection();
        currentVelocity = desiredVelocity;
        UpdateVisualRotation(currentVelocity);
    }

    protected override void OnUpdate()
    {
        UpdateMovement(Time.deltaTime);
    }

    protected override void HandleBulletCollision(Bullet bullet)
    {
        bullet.ReturnToPool();
        GameplayEvents.RaiseEnemyDestroyed(enemyManager.MainConfig.spiderPoints);
        GameplayEvents.RaiseSpiderDestroyed();
        Release();
    }

    protected override void HandlePlayerCollision()
    {
        GameplayEvents.RaisePlayerHit();
    }

    protected override void ProcessCollisionWithOther(Collider2D hitCollider)
    {
        if (hitCollider.TryGetComponent(out MushroomController mushroom))
        {
            gridManager.Release(mushroom);
        }
    }

    private void Release()
    {
        if (isActive)
        {
            isActive = false;
            enemyManager.ReleaseSpider(this);
        }
    }

    private void UpdateMovement(float deltaTime)
    {
        directionTimer += deltaTime;

        if (directionTimer >= directionDuration)
        {
            PickNewMovementDirection();
        }

        float acceleration = baseSpeed * AccelerateMultiplier;
        currentVelocity = Vector2.MoveTowards(currentVelocity, desiredVelocity, acceleration * deltaTime);
        Vector2 newPosition = (Vector2)transform.position + currentVelocity * deltaTime;

        HandleVerticalBounds(ref newPosition);
        transform.position = newPosition;
        UpdateVisualRotation(currentVelocity);
        CheckScreenExit();
    }

    private void PickNewMovementDirection() // picks a new desired velocity
    {
        directionTimer = 0f;
        directionDuration = CalculateNewDirectionDuration();
        Vector2 movementVector = CalculateNewMovementVector();
        desiredVelocity = movementVector * baseSpeed;
        desiredVelocity.x = Mathf.Abs(desiredVelocity.x) * horizontalSign;
    }

    private void ConfigureSpeed()
    {
        Vector2 speedRange = spiderConfig.horizontalSpeedRange;
        baseSpeed = Random.Range(speedRange.x, speedRange.y);
        baseSpeed *= spiderConfig.speedMultiplier;
    }

    private void ConfigureMovementBoundaries()
    {
        Vector2 verticalRange = spiderConfig.verticalRange;
        minY = verticalRange.x;
        maxY = verticalRange.y;
    }

    private float CalculateNewDirectionDuration()
    {
        Vector2 periodRange = spiderConfig.wavePeriodRange;
        return Random.Range(periodRange.x, periodRange.y);
    }

    private Vector2 CalculateNewMovementVector() // picks a new direction within angle around forward
    {
        float maxAngle = spiderConfig.movementAngleRange;
        float randomAngle = Random.Range(-maxAngle, maxAngle);
        Vector2 forwardDirection = Vector2.right;
        return Quaternion.Euler(0, 0, randomAngle) * forwardDirection;
    }

    private void HandleVerticalBounds(ref Vector2 nextPosition)
    {
        if (nextPosition.y < minY || nextPosition.y > maxY)
        {
            currentVelocity.y *= -0.8f;
            desiredVelocity.y *= -1f;
            nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);
        }
    }

    private void CheckScreenExit()
    {
        float padding = spiderConfig.exitPadding;

        if ((horizontalSign < 0 && transform.position.x < enemyManager.MainConfig.leftLimit - padding) ||
            (horizontalSign > 0 && transform.position.x > enemyManager.MainConfig.rightLimit + padding))
        {
            Release();
        }
    }

    private void UpdateVisualRotation(Vector2 velocity) // points sprite toward movement direction
    {
        if (velocity.sqrMagnitude > Mathf.Epsilon)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90f;
            spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void LateUpdate() // sort by y for correct layering
    {

        if (isActive)
        {

            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100f);
        }
    }
}