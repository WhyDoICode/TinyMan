using UnityEngine;

public class KillVolume : MonoBehaviour
{
    // MAKING SURE CHARACTER GETS DESTROYED
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            Destroy(collision.gameObject);
        }
    }
    // This function is called when another collider makes contact with the collider attached to the GameObject this script is on
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            Destroy(collision.gameObject);
        }
    }
}