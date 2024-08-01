using System.Collections;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Vector3 offset;
    public GameObject mySprite;
    public float animationDuration = 0.2f;
    public Vector3 scaleUpSize = new Vector3(1.2f, 1.2f, 1.2f);

    // Update is called once per frame
    void Update()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // Check if the raycast hit this sprite
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Obstacle"))
            {
                mySprite = hit.collider.gameObject;
                offset = mySprite.transform.position - mousePosition;

                // Start the scale animation
                StartCoroutine(ScaleAnimation(mySprite));
            }
        }

        // Check if the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            mySprite = null;
        }

        if (mySprite)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mySprite.transform.position = mousePosition + offset;
        }
    }

    private IEnumerator ScaleAnimation(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;

        // Scale up
        float elapsedTime = 0;
        while (elapsedTime < animationDuration / 2)
        {
            target.transform.localScale = Vector3.Lerp(originalScale, scaleUpSize, elapsedTime / (animationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.transform.localScale = scaleUpSize;

        // Scale down
        elapsedTime = 0;
        while (elapsedTime < animationDuration / 2)
        {
            target.transform.localScale = Vector3.Lerp(scaleUpSize, originalScale, elapsedTime / (animationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.transform.localScale = originalScale;
    }
}
