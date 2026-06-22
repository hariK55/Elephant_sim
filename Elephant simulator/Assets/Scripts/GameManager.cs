using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField]
    private TMP_Text playTime;

   
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI resultText;
   
    void Start()
    {
       
        Time.timeScale = 1f;
        endPanel.SetActive(false);
        
    }

    public void WinGame()
    {
        
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlayMusic(Music.Victory, 2f);
        ShowEndScreen("YOU WIN!");
        resultText.color = Color.yellow;
    }

 
    public void LoseGame(float delay,string msg)
    {
       
        SoundManager.Instance.PlayMusic(Music.loseMusic, 0.7f);
        StartCoroutine(LoseGameRoutine(delay,msg));
    }
    private IEnumerator LoseGameRoutine(float delay, string msg)
    {
        Input.Instance.caught = true;
        yield return new WaitForSeconds(delay);
       // SoundManager.Instance.PlayMusic(Music.loseMusic, 0.7f);
        ShowEndScreen(msg);
        
    }
    void ShowEndScreen(string message)
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
        endPanel.SetActive(true);
        resultText.text = message;
        playTime.text = "Survived Time: "+ GameTimeManager.Instance.GetFormattedTime();
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

    public void PlayButtonSound()
    {
        buttonSound.Play();
    }
    public void HideEndScreen()
    {
        endPanel.SetActive(false);
    }
}
