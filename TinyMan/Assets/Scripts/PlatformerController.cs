using System;
using UnityEngine;

public class PlatformerController : MonoBehaviour
{
    // Movement Settings
    [Header("Movement Settings")]
    [SerializeField, Range(1f, 10f)]
    private float moveSpeed = 5f;

    [SerializeField, Range(1f, 20f)]
    private float jumpForce = 10f;

    [SerializeField, Range(1f, 10f)]
    private float maxJumpHeight = 4f;

    [SerializeField, Range(0.5f, 5f)]
    private float minJumpHeight = 1f;

    [SerializeField, Range(0.1f, 5f)]
    private float detectionRange = 2f;

    // Character State
    [Header("Character State")]
    [SerializeField]
    private bool canMove = true;

    private bool characterInFront = false;
    private bool isGrounded;
    private bool isPerforming = false;

    // Direction
    [Header("Direction")]
    [SerializeField]
    private Vector2 direction;

    // Layer Masks
    [Header("Layer Masks")]
    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private LayerMask excOnGround;

    [SerializeField]
    private LayerMask excInAir;

    // Ground Detection
    [Header("Ground Detection")]
    [SerializeField, Range(0.1f, 0.5f)]
    private float groundCheckDistance = 0.1f;

    [SerializeField, Range(0.01f, 0.5f)]
    private float groundCheckWidth = 0.01f;

    // Components
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private Collider2D collider;
    [SerializeField]
    private Animator animator;

    // Properties
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float MaxJumpHeight => maxJumpHeight;
    public float MinJumpHeight => minJumpHeight;
    public float DetectionRange => detectionRange;
    public bool CanMove => canMove;
    public bool CharacterInFront => characterInFront;
    public bool IsGrounded => isGrounded;
    public Vector2 Direction => direction;
    public bool IsPerforming => isPerforming;

    private void Awake()
    {

    }

    private void Update()
    {
        HandleInput();
        LookAhead();
        UpdateAnimator();
        CheckGround();

        if (canMove) Move();
    }

    private void CheckGround()
    {
        // Cast a short ray downwards from the center of the collider to detect ground
        Vector2 origin = (Vector2)transform.position + Vector2.down * (collider.bounds.extents.y);
        RaycastHit2D hitCenter = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);

        // Cast two additional rays at the left and right edges of the collider
        Vector2 leftOrigin = origin + Vector2.left * (collider.bounds.extents.x - groundCheckWidth);
        Vector2 rightOrigin = origin + Vector2.right * (collider.bounds.extents.x - groundCheckWidth);

        RaycastHit2D hitLeft = Physics2D.Raycast(leftOrigin, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rightOrigin, Vector2.down, groundCheckDistance, groundLayer);

        isGrounded = hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;

        if (isGrounded)
        {
            canMove = true;
        }

        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, Color.red);
        Debug.DrawRay(leftOrigin, Vector2.down * groundCheckDistance, Color.red);
        Debug.DrawRay(rightOrigin, Vector2.down * groundCheckDistance, Color.red);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReduceCharacterCount();
        }
    }

    private void LookAhead()
    {
        // Raycast in the direction the character is facing
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange);

        if (hit.collider != null && !hit.collider.gameObject.CompareTag("Obstacle") && !hit.collider.gameObject.CompareTag("Kill"))
        {
            canMove = false;
            characterInFront = true;
        }
        else
        {
            canMove = true;
            characterInFront = false;
        }
    }

    private void HandleInput()
    {
        // Example input handling
    }

    private void Move()
    {
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
            float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * maxJumpHeight);
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            isGrounded = false;
            animator.SetTrigger("JumpTrig");
    }

    public void JumpWithForce(float jumpHeight)
    {
            float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            isGrounded = false;
            animator.SetTrigger("JumpTrig");
    }

    public void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        direction = Vector2.zero;
    }

    public void ChangeDirection(Vector2 newDirection)
    {
        direction = newDirection;
        float mainDir = 0.6f * direction.x;
        transform.localScale = new Vector3(mainDir, 0.6f, 0.6f);
    }

    public void RotateDirection()
    {
        direction.x *= -1;
        float mainDir = 0.6f * direction.x;
        transform.localScale = new Vector3(mainDir, 0.6f, 0.6f);
    }

    public void RotateDirectionAndJump(float jumpHeight)
    {
        RotateDirection();
        JumpWithForce(jumpHeight);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isPerforming)
        {
            var entity = collision.gameObject.GetComponent<ObstacleEntity>();
            if (entity != null)
            {
                isPerforming = true;
                entity.PerformObstacleAction(this);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != null && collision.gameObject.CompareTag("Obstacle"))
        {
            var entity = collision.gameObject.GetComponent<ObstacleEntity>();
            if (entity != null)
            {
                isPerforming = true;
                entity.PerformObstacleAction(this);
            }
            else
            {
                if (!isGrounded)
                {
                    canMove = false;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject != null && collision.gameObject.CompareTag("Obstacle"))
        {
            isPerforming = false;
        }
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("Jump", !isGrounded); // Set Jump to true if not grounded
            animator.SetBool("IsRunning", isGrounded); // Set Running based on movement
        }
    }
}