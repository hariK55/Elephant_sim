using System.Collections;
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

  /*  public void LoseGame()
    {
        ShowEndScreen("YOU LOSE!");
       Input.Instance.caught= true;
    }*/
    public void LoseGame(float delay,string msg)
    {
        StartCoroutine(LoseGameRoutine(delay,msg));
    }
    private IEnumerator LoseGameRoutine(float delay, string msg)
    {
        Input.Instance.caught = true;
        yield return new WaitForSeconds(delay);

        ShowEndScreen(msg);
        
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
