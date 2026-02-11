using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region singleton
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public GameObject endPanel;
    public TextMeshProUGUI resultText;   // If using TextMeshPro use TMP_Text

    void Start()
    {
        Time.timeScale = 1f;
        endPanel.SetActive(false);
    }

    public void WinGame()
    {
        ShowEndScreen("YOU WIN!");
    }

    public void LoseGame()
    {
        ShowEndScreen("YOU LOSE!");
    }

    void ShowEndScreen(string message)
    {
        endPanel.SetActive(true);
        resultText.text = message;
        Time.timeScale = 0f; // pause game
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
