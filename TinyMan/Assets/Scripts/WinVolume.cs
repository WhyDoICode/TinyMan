using UnityEngine;

public class WinVolume : MonoBehaviour
{
    // MAKING SURE CHARACTER GETS DESTROYED ON ENTER
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.IncrementWinCount();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.IncrementWinCount();
        }
    }
}