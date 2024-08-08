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
    private LayerMask excOnGround;

    [SerializeField]
    private LayerMask excInAir;

    // Components
    private Rigidbody2D rb;
    private Collider2D collider;
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
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        LookAhead();
        UpdateAnimator();

        if (canMove) Move();
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
        direction.x *= -1;
        float mainDir = 0.6f * direction.x;
        transform.localScale = new Vector3(mainDir, 0.6f, 0.6f);
        Debug.Log(direction.x.ToString());
        JumpWithForce(jumpHeight);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isPerforming)
        {
            Debug.Log("STUCK");
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
        if (collision.contacts.Length > 0)
        {
            ContactPoint2D contact = collision.contacts[0];
            if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
            {
                isGrounded = true;
            }
        }

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
        if (collision.contacts.Length > 0)
        {
            ContactPoint2D contact = collision.contacts[0];

            if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
            {
                isGrounded = false;
                Debug.Log("Not Grounded");
            }
        }

        if (collision.gameObject != null && collision.gameObject.CompareTag("Obstacle"))
        {
            isPerforming = false;
        }
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            if (!isGrounded)
            {
                if (rb != null) rb.excludeLayers = excInAir;
                if (collider != null) collider.excludeLayers = excInAir;
            }
            else
            {
                if (rb != null) rb.excludeLayers = excOnGround;
                if (collider != null) collider.excludeLayers = excOnGround;
            }
            animator.SetBool("Jump", !isGrounded); // Set Jump to true if not grounded
            animator.SetBool("IsRunning", isGrounded); // Set Running based on movement
        }
    }
}
