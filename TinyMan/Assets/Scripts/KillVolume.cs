using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("A");

        // Check if the object colliding has the tag "Character"
        if (collision.gameObject.CompareTag("Character"))
        {
            Debug.Log("H");
            // Destroy the game object involved in the collision
            Destroy(collision.gameObject);
        }
    }
    // This function is called when another collider makes contact with the collider attached to the GameObject this script is on
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("A");

        // Check if the object colliding has the tag "Character"
        if (collision.gameObject.CompareTag("Character"))
        {
             Debug.Log("H");
            // Destroy the game object involved in the collision
            Destroy(collision.gameObject);
        }
    }
}