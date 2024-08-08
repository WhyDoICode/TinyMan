using System.Collections;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField, Range(0.1f, 1f)]
    private float _animationDuration = 0.2f;
    [SerializeField]
    private Vector3 _scaleUpSize = new Vector3(1.2f, 1.2f, 1.2f);

    private Vector3 _offset;
    private GameObject _selectedSprite;

    public float AnimationDuration => _animationDuration;
    public Vector3 ScaleUpSize => _scaleUpSize;

    private void Update()
    {
        HandleMouseInput();
        if (_selectedSprite != null)
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
            _selectedSprite = hit.collider.gameObject;
            _offset = _selectedSprite.transform.position - mousePosition;

            StartCoroutine(ScaleAnimation(_selectedSprite));
        }
    }

    private void DeselectSprite()
    {
        _selectedSprite = null;
    }

    private void MoveSelectedSprite()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _selectedSprite.transform.position = mousePosition + _offset;
    }

    private IEnumerator ScaleAnimation(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;

        float elapsedTime = 0f;
        while (elapsedTime < _animationDuration / 2)
        {
            target.transform.localScale = Vector3.Lerp(originalScale, _scaleUpSize, elapsedTime / (_animationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.transform.localScale = _scaleUpSize;

        elapsedTime = 0f;
        while (elapsedTime < _animationDuration / 2)
        {
            target.transform.localScale = Vector3.Lerp(_scaleUpSize, originalScale, elapsedTime / (_animationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.transform.localScale = originalScale;
    }
}
