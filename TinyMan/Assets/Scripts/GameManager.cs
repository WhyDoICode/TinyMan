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

    public GameState CurrentState { get; private set; } = GameState.Playing;
    [Header("Spawner Management")]
    public Spawner[] Spawners;
    [Header("Character Management")]
    private int _maxCharacters = 9;
    [SerializeField, Range(1, 20)]
    private int _charactersToWin = 4;
    [SerializeField, Range(0.1f, 5f)]
    private float _respawnDelay = 0.3f;
    private int _totalCharacters;
    private int _limitToWin;
    private int _charactersInWinVolume = 0;

    [Header("UI Elements")]
    public RectTransform LoseUI;
    public RectTransform WinUI;
    public TextMeshProUGUI CharacterCountText;
    public TextMeshProUGUI LimitText;

    [SerializeField, Range(0.1f, 5f)]
    private float _fadeDuration = 1f;

    private bool _isRespawning = false;
    private bool _isRestarting = false;

    private void Awake()
    {
        ImplementSingletonPattern();
        InitializeValues();
    }

    private void InitializeValues()
    {
        _totalCharacters = _maxCharacters;
        _limitToWin = _charactersToWin;
    }

    private void Start()
    {
        _maxCharacters = Spawners.Length;
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
        LimitText.text = _limitToWin.ToString();
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
        if (_charactersInWinVolume >= _limitToWin)
        {
            TriggerWin();
        }
    }

    private void RespawnCharacters()
    {
        if (_isRespawning) return;

        _isRespawning = true;
        InitializeValues();
        UpdateCharacterCountText();
        foreach (Spawner spawner in Spawners)
        {
            spawner?.CreateEntity();
        }

        StartCoroutine(EndRespawn(_respawnDelay));
    }

    private IEnumerator EndRespawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        _isRespawning = false;
    }

    public void ReduceCharacterCount()
    {
        if (_isRespawning) return;
        
        _totalCharacters--;
        UpdateCharacterCountText();

        if (_totalCharacters < _limitToWin)
        {
            RespawnCharacters();
        }
    }

    private void UpdateCharacterCountText()
    {
        if (CharacterCountText != null)
        {
            CharacterCountText.text = _totalCharacters.ToString();
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

    public void IncrementWinCount() { _charactersInWinVolume++; }
    
    
    private void RestartGame()
    {
        if (_isRestarting) return;

        _isRestarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        CurrentState = GameState.Playing;
    }

    private void SetAlpha(RectTransform rectTransform, float alpha)
    {
        if (rectTransform.TryGetComponent(out CanvasRenderer canvasRenderer))
        {
            canvasRenderer.SetAlpha(alpha);
        }
    }
}
