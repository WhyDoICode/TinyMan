using UnityEngine;

public class PlatformerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(1f, 10f)]
    private float _moveSpeed = 5f;
    [SerializeField, Range(1f, 20f)]
    private float _jumpForce = 10f;
    [SerializeField, Range(1f, 10f)]
    private float _maxJumpHeight = 4f;
    [SerializeField, Range(0.5f, 5f)]
    private float _minJumpHeight = 1f;
    [SerializeField, Range(0.1f, 5f)]
    private float _detectionRange = 2f;

    [Header("Character State")]
    [SerializeField]
    private bool _canMove = true;

    private bool _characterInFront = false;
    private bool _isGrounded;
    private bool _isPerforming = false;

    [Header("Direction")]
    [SerializeField]
    private Vector2 _direction;

    [Header("Layer Masks")]
    [SerializeField]
    private LayerMask _groundLayer;

    [Header("Ground Detection")]
    [SerializeField, Range(0.1f, 0.5f)]
    private float _groundCheckDistance = 0.1f;
    [SerializeField, Range(0.01f, 0.5f)]
    private float _groundCheckWidth = 0.01f;

    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private Collider2D _collider;
    [SerializeField]
    private Animator _animator;

    public float MoveSpeed => _moveSpeed;
    public float JumpForce => _jumpForce;
    public float MaxJumpHeight => _maxJumpHeight;
    public float MinJumpHeight => _minJumpHeight;
    public float DetectionRange => _detectionRange;
    public bool CanMove => _canMove;
    public bool CharacterInFront => _characterInFront;
    public bool IsGrounded => _isGrounded;
    public Vector2 Direction => _direction;
    public bool IsPerforming => _isPerforming;

    private void Update()
    {
        HandleInput();
        LookAhead();
        UpdateAnimator();
        CheckGround();

        if (_canMove) Move();
    }

    private void CheckGround()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * (_collider.bounds.extents.y);
        RaycastHit2D hitCenter = Physics2D.Raycast(origin, Vector2.down, _groundCheckDistance, _groundLayer);

        Vector2 leftOrigin = origin + Vector2.left * (_collider.bounds.extents.x - _groundCheckWidth);
        Vector2 rightOrigin = origin + Vector2.right * (_collider.bounds.extents.x - _groundCheckWidth);

        RaycastHit2D hitLeft = Physics2D.Raycast(leftOrigin, Vector2.down, _groundCheckDistance, _groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rightOrigin, Vector2.down, _groundCheckDistance, _groundLayer);

        _isGrounded = hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;

        if (_isGrounded)
        {
            _canMove = true;
        }

        Debug.DrawRay(origin, Vector2.down * _groundCheckDistance, Color.red);
        Debug.DrawRay(leftOrigin, Vector2.down * _groundCheckDistance, Color.red);
        Debug.DrawRay(rightOrigin, Vector2.down * _groundCheckDistance, Color.red);
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, _detectionRange);

        if (hit.collider != null && !hit.collider.gameObject.CompareTag("Obstacle") && !hit.collider.gameObject.CompareTag("Kill"))
        {
            _canMove = false;
            _characterInFront = true;
        }
        else
        {
            _canMove = true;
            _characterInFront = false;
        }
    }

    private void HandleInput()
    {
        // Example input handling
    }

    private void Move()
    {
        _rb.velocity = new Vector2(_direction.x * _moveSpeed, _rb.velocity.y);
    }

    private void Jump()
    {
        float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * _maxJumpHeight);
        _rb.velocity = new Vector2(_rb.velocity.x, jumpVelocity);
        _isGrounded = false;
        _animator.SetTrigger("JumpTrig");
    }

    public void JumpWithForce(float jumpHeight)
    {
        float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);
        _rb.velocity = new Vector2(_rb.velocity.x, jumpVelocity);
        _isGrounded = false;
        _animator.SetTrigger("JumpTrig");
    }

    public void Stop()
    {
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        _direction = Vector2.zero;
    }

    public void ChangeDirection(Vector2 newDirection)
    {
        _direction = newDirection;
        float mainDir = 0.6f * _direction.x;
        transform.localScale = new Vector3(mainDir, 0.6f, 0.6f);
    }

    public void RotateDirection()
    {
        _direction.x *= -1;
        float mainDir = 0.6f * _direction.x;
        transform.localScale = new Vector3(mainDir, 0.6f, 0.6f);
    }

    public void RotateDirectionAndJump(float jumpHeight)
    {
        RotateDirection();
        JumpWithForce(jumpHeight);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!_isPerforming)
        {
            var entity = collision.gameObject.GetComponent<ObstacleEntity>();
            if (entity != null)
            {
                _isPerforming = true;
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
                _isPerforming = true;
                entity.PerformObstacleAction(this);
            }
            else
            {
                if (!_isGrounded)
                {
                    _canMove = false;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject != null && collision.gameObject.CompareTag("Obstacle"))
        {
            _isPerforming = false;
        }
    }

    private void UpdateAnimator()
    {
        if (_animator != null)
        {
            _animator.SetBool("Jump", !_isGrounded);
            _animator.SetBool("IsRunning", _isGrounded);
        }
    }
}
