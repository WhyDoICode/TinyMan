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
    public bool CharacterInFront = false;
    private bool isGrounded;
    public Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
        LookAhead();
        if(bCanMove) Move();
    }

    void LookAhead()
    {
        // Raycast in the direction the character is facing
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange);

        if (hit.collider != null && hit.collider.CompareTag("Character") && hit.collider.gameObject != this.gameObject)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
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
    }

    public void JumpWithForce(float jumpHeight)
    {
        float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        isGrounded = false;
    }

    public void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        direction = Vector2.zero;
    }

    public void ChangeDirection(Vector2 newDirection)
    {
        direction = newDirection;
        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1); // Flip character sprite
    }

    public void RotateDirection()
    {
        direction.x -= direction.x;
    }
    
    public void RotateDirectionAndJump(float jumpHeight)
    {
        direction.x *= -1;
        Debug.Log(direction.x.ToString());
        JumpWithForce(jumpHeight);
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
            }
        }
    }
}
