using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    [SerializeField] private GameObject firstButton;
    [SerializeField] private AudioSource buttonSound;

    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject backButton;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
        credits.SetActive(false);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("LoadScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayButtonSound()
    {
        buttonSound.Play();
    }

    public void PlayCredits()
    {
        EventSystem.current.SetSelectedGameObject(backButton);
        credits.SetActive(true);
    }

    public void StopCredits()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
        credits.SetActive(false);
    }
}
