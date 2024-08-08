using UnityEngine;

public class RandomizedLocation : MonoBehaviour
{
    // Position range settings
    [Header("Random Position Range")]
    [SerializeField, Range(-10f, 10f)]
    private float minX = -5f;

    [SerializeField, Range(-10f, 10f)]
    private float maxX = 5f;

    [SerializeField, Range(-10f, 10f)]
    private float minY = -5f;

    [SerializeField, Range(-10f, 10f)]
    private float maxY = 5f;

    private void Start()
    {
        RandomizePosition();
    }

    private void RandomizePosition()
    {
        // Generate random X and Y positions within the specified ranges
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        // Set the game object's local position to the new random position relative to its parent
        transform.localPosition = new Vector3(randomX, randomY, transform.localPosition.z);
    }
}