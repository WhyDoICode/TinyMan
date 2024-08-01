using UnityEngine;

public class PlatformerController : MonoBehaviour
{
    // Movement settings
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    
    public float detectionRange = 2f; // Range for detecting characters in front
    public bool bCanMove = true;
    private Rigidbody2D rb;
    private Collider2D cd;
    private Animator animator;
    public bool CharacterInFront = false;
    public bool isGrounded;
    public Vector2 direction;
    public LayerMask ExcOnGround;
    public LayerMask ExcInAir;
    public bool IsPerforming = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        HandleInput();
        LookAhead();
        UpdateAnimator(); // Update animator based on current states

        if (bCanMove) Move();
    }

    void LookAhead()
    {
        // Raycast in the direction the character is facing
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange);

        if (hit.collider != null && !hit.collider.gameObject.CompareTag("Obstacle"))
        {
            
            bCanMove = false;
            CharacterInFront = true;
        }
        else
        {
            bCanMove = true;
            CharacterInFront = false;
        }
    }

    void HandleInput()
    {
        // Example input handling
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    void Jump()
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
        this.gameObject.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    public void RotateDirection()
    {
        direction.x *= -1;
        this.gameObject.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }
    
    public void RotateDirectionAndJump(float jumpHeight)
    {
        direction.x *= -1;
        this.gameObject.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        Debug.Log(direction.x.ToString());
        JumpWithForce(jumpHeight);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsPerforming)
        {
            Debug.Log("STUCK");
            var Entity = collision.gameObject.GetComponent<ObstacleEntity>();
            if(Entity)
            {
                IsPerforming = true;
                Entity.PerformObstacleAction(this);
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
            var Entity = collision.gameObject.GetComponent<ObstacleEntity>();
            if(Entity)
            {
                IsPerforming = true;
                Entity.PerformObstacleAction(this);
            }
            else
            {
                if (!isGrounded)
                {
                    bCanMove = false;
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
            IsPerforming = false;
        }
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            if (!isGrounded)
            {
                if (rb != null) rb.excludeLayers = ExcInAir;
                if (cd != null) cd.excludeLayers = ExcInAir;
            }
            else
            {
                if (rb != null) rb.excludeLayers = ExcOnGround;
                if (cd != null) cd.excludeLayers = ExcOnGround;
            }
            animator.SetBool("Jump", !isGrounded); // Set Jump to true if not grounded
            animator.SetBool("IsRunning", isGrounded); // Set Running based on movement
        }
    }
}
