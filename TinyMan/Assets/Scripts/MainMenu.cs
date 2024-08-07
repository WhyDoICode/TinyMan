using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load scenes

public class MainMenu : MonoBehaviour
{
    public string GameName;
    // This method is called when the Start Game button is clicked
    public void StartGame()
    {
        // Load the game scene; ensure the correct scene index or name is used
        SceneManager.LoadScene(GameName); // Replace "GameScene" with your actual game scene name or index
    }

    // This method is called when the Quit Game button is clicked
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // This will only show in the editor
        Application.Quit(); // Quits the application; this won't work in the editor
    }
}