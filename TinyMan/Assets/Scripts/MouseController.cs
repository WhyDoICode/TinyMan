using Unity.VisualScripting;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    public ObstacleController Obstacle;

    void Start()
    {
        // Initialize the Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        // Initialize the movement direction to the current up direction
        moveDirection = transform.up;
    }

    void Update()
    {
        // Rotate the object to face the movement direction
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Adjust the angle based on the initial orientation
    }

    void FixedUpdate()
    {
        // Move the object in the current direction using Rigidbody2D
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Obstacle != null && collision.gameObject != Obstacle.mySprite)
        {
            Debug.Log("Collision detected with " + collision.gameObject.name);
            SetRandomDirection();
        }
        
        if (Obstacle != null && (collision.gameObject.CompareTag("Collectable")))
        {
            Destroy(collision.gameObject);
        }
        
        if (Obstacle != null && (collision.gameObject.CompareTag("Trap")))
        {
            Destroy(this.gameObject);
        }
    }

    private void SetRandomDirection()
    {
        float randomAngle = Random.Range(-45f, 45f);

        // Calculate the new direction
        moveDirection = Quaternion.Euler(0, 0, randomAngle) * -moveDirection;
        moveDirection.Normalize(); // Ensure the direction vector is normalized
    }
}