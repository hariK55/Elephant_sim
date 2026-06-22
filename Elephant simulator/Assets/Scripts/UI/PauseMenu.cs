using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject strtTxt;


    [SerializeField] private GameObject firstButton;

    public GameObject pauseMenuUI;

    private InputSystem inputActions;
    private bool isPaused = false;

    private void Awake()
    {
        inputActions = new InputSystem();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Pause.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        inputActions.Player.Pause.performed -= OnPausePressed;
        inputActions.Player.Disable();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SoundManager.Instance.PlayMusic(Music.pauseFX, 0.5f);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Pause()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
        SoundManager.Instance.PlayMusic(Music.pauseFX, 0.5f);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}