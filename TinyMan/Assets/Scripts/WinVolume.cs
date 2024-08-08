using UnityEngine;

public class WinVolume : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void ProcessCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.IncrementWinCount();
        }
    }
}