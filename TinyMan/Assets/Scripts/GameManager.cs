using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        Playing,
        Won,
        Lost
    }

    // Game State Management
    public GameState CurrentState { get; private set; } = GameState.Playing;

    // Spawner Management
    [Header("Spawner Management")]
    public Spawner[] Spawners;

    // Character Management
    [Header("Character Management")]
    private int maxCharacters = 9;

    [SerializeField, Range(1, 20)]
    private int charactersToWin = 4;

    [SerializeField, Range(0.1f, 5f)]
    private float respawnDelay = 0.3f;

    private int totalCharacters;
    private int limitToWin;
    private int charactersInWinVolume = 0; // Characters currently in the win volume

    // UI Elements
    [Header("UI Elements")]
    public RectTransform LoseUI;
    public RectTransform WinUI;
    public TextMeshProUGUI CharacterCountText;
    public TextMeshProUGUI LimitText;

    [SerializeField, Range(0.1f, 5f)]
    private float fadeDuration = 1f;


    private bool isRespawning = false;
    private bool isRestarting = false;

    private void Awake()
    {
        ImplementSingletonPattern();

        InitializeValues();
    }

    void InitializeValues()
    {
        totalCharacters = maxCharacters;
        limitToWin = charactersToWin;
    }

    private void Start()
    {
        maxCharacters = Spawners.Length;
        InitializeUI();
        RespawnCharacters();
    }

    private void Update()
    {
        CheckForRestartInput();
    }

    private void FixedUpdate()
    {
        CheckWinCondition();
    }

    private void ImplementSingletonPattern()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeUI()
    {
        LimitText.text = limitToWin.ToString();
        UpdateCharacterCountText();
        SetAlpha(LoseUI, 0f);
        WinUI.gameObject.SetActive(false);
    }

    private void CheckForRestartInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnCharacters();
        }
    }

    private void CheckWinCondition()
    {
        if (charactersInWinVolume >= limitToWin)
        {
            TriggerWin();
        }
    }

    private void RespawnCharacters()
    {
        if (isRespawning) return;

        isRespawning = true;
        InitializeValues();
        UpdateCharacterCountText();
        foreach (Spawner spawner in Spawners)
        {
            spawner?.CreateEntity();
        }

        StartCoroutine(EndRespawn(respawnDelay));
    }

    private IEnumerator EndRespawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        isRespawning = false;
    }

    public void ReduceCharacterCount()
    {
        if (isRespawning) return;
        
        totalCharacters--;
        UpdateCharacterCountText();

        if (totalCharacters < limitToWin)
        {
            RespawnCharacters();
        }
    }

    private void UpdateCharacterCountText()
    {
        if (CharacterCountText != null)
        {
            CharacterCountText.text = totalCharacters.ToString();
        }
    }

    private void TriggerWin()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Won;
        Debug.Log("You win!");
        StartCoroutine(ShowWinUI());
    }

    private IEnumerator ShowWinUI()
    {

        SetAlpha(WinUI, 1f);
        WinUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        RestartGame();
    }

    public void IncrementWinCount() { charactersInWinVolume ++; }
    
    
    private void RestartGame()
    {
        if (isRestarting) return;

        isRestarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        CurrentState = GameState.Playing;
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
