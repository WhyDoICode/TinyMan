using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load scenes

public class MainMenu : MonoBehaviour
{
    public string GameName;

    public void StartGame()
    {
        SceneManager.LoadScene(GameName); 
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game"); 
        Application.Quit(); 
    }
}