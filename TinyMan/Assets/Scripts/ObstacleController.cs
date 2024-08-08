using System.Collections;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    // Animation Settings
    [Header("Animation Settings")]
    [SerializeField, Range(0.1f, 1f)]
    private float animationDuration = 0.2f;

    [SerializeField]
    private Vector3 scaleUpSize = new Vector3(1.2f, 1.2f, 1.2f);

    // Interaction
    private Vector3 offset;
    private GameObject selectedSprite;

    // Properties
    public float AnimationDuration => animationDuration;
    public Vector3 ScaleUpSize => scaleUpSize;

    private void Update()
    {
        HandleMouseInput();
        if (selectedSprite != null)
        {
            MoveSelectedSprite();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectSpriteUnderMouse();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DeselectSprite();
        }
    }

    private void SelectSpriteUnderMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Obstacle"))
        {
            selectedSprite = hit.collider.gameObject;
            offset = selectedSprite.transform.position - mousePosition;

            StartCoroutine(ScaleAnimation(selectedSprite));
        }
    }

    private void DeselectSprite()
    {
        selectedSprite = null;
    }

    private void MoveSelectedSprite()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectedSprite.transform.position = mousePosition + offset;
    }

    private IEnumerator ScaleAnimation(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;

        // Scale up
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration / 2)
        {
            target.transform.localScale = Vector3.Lerp(originalScale, scaleUpSize, elapsedTime / (animationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.transform.localScale = scaleUpSize;

        // Scale down
        elapsedTime = 0f;
        while (elapsedTime < animationDuration / 2)
        {
            target.transform.localScale = Vector3.Lerp(scaleUpSize, originalScale, elapsedTime / (animationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.transform.localScale = originalScale;
    }
}
