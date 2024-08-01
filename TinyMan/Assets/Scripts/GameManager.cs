using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Game state
    public enum GameState
    {
        Playing,
        Won,
        Lost
    }

    public GameState currentState = GameState.Playing;

    // Character management
    public int TotalCharacters;
    public int LimitToWin;

    // UI Elements
    public RectTransform loseRectTransform; // UI element for losing
    public TextMeshProUGUI characterCountText; // UI text to display total characters
    public float fadeDuration = 1f; // Duration for fade-in and fade-out

    public Spawner[] spawners; // Array of spawners

    private bool IsRespawning = false;
    // Method to call CreateEntity on each spawner
    public void Respawn()
    {
        IsRespawning = true;
        foreach (Spawner spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.CreateEntity();
            }
        }
    }
    private void Awake()
    {
        // Implement the singleton pattern
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Persist the GameManager across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate GameManager instances
        }
    }

    private void Start()
    {
        Respawn();
        UpdateCharacterCountText(); // Initialize the character count display
        SetAlpha(loseRectTransform, 0f); // Initially hide the lose UI
    }

    private void Update()
    {
        // Check for restart input
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    // Call this method to declare the player has won
    public void WinGame()
    {
        if (currentState != GameState.Playing) return; // Only transition if currently playing
        currentState = GameState.Won;
        Debug.Log("You win!");
        StartCoroutine(RestartAfterDelay(0.3f)); // Restart the game after a short delay
    }

    // Call this method to declare the player has lost
    public void LoseGame()
    {
        if (currentState != GameState.Playing) return; // Only transition if currently playing
        currentState = GameState.Lost;
        Debug.Log("You lose!");

        // Start the fade-in and fade-out process
        StartCoroutine(FadeLoseUI());
    }
    bool restarting;

    // Method to restart the current game/scene
    private void RestartGame()
    {
        if (restarting == true) return;
        restarting = true;
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Reset the game state
        currentState = GameState.Playing;
    }

    // Coroutine to restart the game after a delay
    private IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RestartGame();
    }
    
    private IEnumerator RespawnEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsRespawning = false;
    }


    // Reduce character count and check for losing condition
    public void ReduceCharacter()
    {
        TotalCharacters--;
        UpdateCharacterCountText(); // Update the UI text when character count changes

        if (TotalCharacters < LimitToWin)
        {
            RestartGame();
        }
    }

    // Update the UI text to display the total number of characters
    private void UpdateCharacterCountText()
    {
        if (characterCountText != null)
        {
            characterCountText.text = "Characters Left: " + TotalCharacters;
        }
    }

    // Coroutine to handle the fade-in and fade-out effect
    private IEnumerator FadeLoseUI()
    {
        // Fade in
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            SetAlpha(loseRectTransform, Mathf.Lerp(0f, 1f, timer / fadeDuration));
            yield return null;
        }

        // Pause for effect
        yield return new WaitForSeconds(1f);

        // Fade out
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            SetAlpha(loseRectTransform, Mathf.Lerp(1f, 0f, timer / fadeDuration));
            yield return null;
        }

        // After fade out, restart game
        StartCoroutine(RestartAfterDelay(2f));
    }

    // Helper method to set alpha of a RectTransform using CanvasRenderer
    private void SetAlpha(RectTransform rectTransform, float alpha)
    {
        if (rectTransform.TryGetComponent(out CanvasRenderer canvasRenderer))
        {
            canvasRenderer.SetAlpha(alpha);
        }
    }
}
