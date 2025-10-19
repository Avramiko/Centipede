using System.Collections;
using UnityEngine;

public class GnomeController : Singleton<GnomeController>
{
    [Header("Configuration")]
    [SerializeField] private GnomeConfig gnomeConfig;

    [Header("Component References")]
    [SerializeField] private Rigidbody2D rigidBodyGnome;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D gnomeCollider;
    [SerializeField] private Transform firePoint;

    private BestObjectPool<Bullet> bulletPool;
    private Coroutine blinkRoutine;
    private Vector2 spawnPosition;
    private Vector2 moveInput;
    private float nextFireTime;
    private bool controlsEnabled = true;

    public GnomeConfig Config => gnomeConfig;

    protected void Awake()
    {
        bulletPool = new BestObjectPool<Bullet>(gnomeConfig.bulletPrefab, 10);
        spawnPosition = transform.position;
    }

    private void OnEnable()
    {
        GameplayEvents.BulletReleased += OnBulletReleased;
        GameplayEvents.GameResetRequested += OnGameResetRequested;
        ResetToSpawn();
    }

    private void OnDisable()
    {
        GameplayEvents.BulletReleased -= OnBulletReleased;
        GameplayEvents.GameResetRequested -= OnGameResetRequested;

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            ProcessInput();
            HandleShooting();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public void ResetToSpawn() // resets player to initial position and re-enables control
    {
        transform.position = spawnPosition;
        rigidBodyGnome.position = spawnPosition;
        SetControlsEnabled(true);
        SetCollidersEnabled(true);
        SetSpriteVisible(true);
    }

    public void SetControlsEnabled(bool isEnabled)
    {
        controlsEnabled = isEnabled;

        if (!isEnabled)
        {
            moveInput = Vector2.zero;
        }
    }

    public void SetCollidersEnabled(bool isEnabled)
    {
        gnomeCollider.enabled = isEnabled;
    }

    public void StartInvulnerability(float duration, float interval) // enables blinking & disables collisions temporarily
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
        }

        SetCollidersEnabled(false);
        blinkRoutine = StartCoroutine(BlinkRoutine(duration, interval));
    }

    private void ProcessInput()
    {
        moveInput.x = Input.GetAxisRaw(Constants.HorizontalInput);
        moveInput.y = Input.GetAxisRaw(Constants.VerticalInput);
    }

    private void HandleMovement()
    {
        Vector2 movementVector = moveInput.normalized * gnomeConfig.moveSpeed;
        Vector2 nextPosition = rigidBodyGnome.position + movementVector * Time.fixedDeltaTime;

        nextPosition.x = Mathf.Clamp(nextPosition.x, gnomeConfig.movementBounds.xMin, gnomeConfig.movementBounds.xMax);
        nextPosition.y = Mathf.Clamp(nextPosition.y, gnomeConfig.movementBounds.yMin, gnomeConfig.movementBounds.yMax);

        rigidBodyGnome.MovePosition(nextPosition);
    }

    private void HandleShooting()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + gnomeConfig.fireRate;
        }
    }

    private void Shoot()
    {
        Bullet bullet = bulletPool.GetObject();
        bullet.transform.position = firePoint.position;
        bullet.Activate();
        GameplayEvents.RaiseBulletFired();
    }

    private void OnBulletReleased(Bullet bullet)
    {
        bulletPool.ReleaseObject(bullet);
    }

    private void OnGameResetRequested()
    {
        ResetToSpawn();
    }

    private void SetSpriteVisible(bool isVisible)
    {
        spriteRenderer.enabled = isVisible;
    }

    private IEnumerator BlinkRoutine(float duration, float interval) // alternates sprite visibility to indicate invulnerability
    {
        float timer = 0f;

        while (timer < duration)
        {
            SetSpriteVisible(!spriteRenderer.enabled);
            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        SetSpriteVisible(true);
        SetCollidersEnabled(true);
        blinkRoutine = null;
    }
}