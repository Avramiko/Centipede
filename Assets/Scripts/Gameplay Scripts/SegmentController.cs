using UnityEngine;

public class SegmentController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer segmentSpriteRenderer;

    private CentipedeManager centipedeManager;
    private int chainIndex;
    private int segmentIndex;
    private Vector3 targetPosition;

    private bool isSegmentActive = true;

    public int GetChainIndex() => chainIndex;
    public int GetSegmentIndex() => segmentIndex;

    private void Awake()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        float segmentMoveSpeed = centipedeManager.GetSegmentMoveSpeed();
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, segmentMoveSpeed * Time.deltaTime);
    }

    private void OnEnable()
    {
        transform.rotation = Quaternion.identity;
        isSegmentActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.PlayerTag))
        {
            if (isSegmentActive)
            {
                GameplayEvents.RaisePlayerHit();
            }

            return;
        }

        if (collider.TryGetComponent(out Bullet bullet) && !bullet.IsReturned)
        {
            if (isSegmentActive)
            {
                isSegmentActive = false;
                centipedeManager.HandleSegmentHit(this, bullet);
            }
        }
    }

    public void Setup(CentipedeManager centipedeManager, int chainIndex, int segmentIndex)
    {
        this.centipedeManager = centipedeManager;
        this.chainIndex = chainIndex;
        this.segmentIndex = segmentIndex;
    }

    public void UpdateChainAndSegmentIndex(int newChainIndex, int newSegmentIndex)
    {
        chainIndex = newChainIndex;
        segmentIndex = newSegmentIndex;
    }

    public void SetTargetPosition(Vector2 target)
    {
        targetPosition = new Vector3(target.x, target.y, transform.position.z);
    }

    public void SetAppearance(Sprite segmentSprite, bool shouldFlipHorizontally) // applies sprite and horizontal flip
    {
        segmentSpriteRenderer.sprite = segmentSprite;
        segmentSpriteRenderer.flipX = shouldFlipHorizontally;
    }

    public void SetDiveRotation(bool isVertical, float horizontalDirectionSign) // tilts sprite 90° based on dive direction
    {
        if (!isVertical)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        float angle = (horizontalDirectionSign > 0) ? -90f : 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void LateUpdate() // updates order by y for proper layering
    {
        if (isSegmentActive)
        {
            segmentSpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100f);
        }
    }
}