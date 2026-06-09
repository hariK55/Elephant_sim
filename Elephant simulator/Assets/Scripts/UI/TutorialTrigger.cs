using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject tutorialPanel;
   

    private bool tutorialActive;

    private void Start()
    {
        tutorialPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
       

        if (!other.CompareTag("Player"))
            return;

        tutorialPanel.SetActive(true);
        //tutorialText.text = message;

        tutorialActive = true;
        Time.timeScale = 0f;

        GetComponent<Collider>().enabled = false;
    }

    private void Update()
    {
        if (!tutorialActive)
            return;

        bool keyboardPressed =
            Keyboard.current != null &&
            Keyboard.current.anyKey.wasPressedThisFrame;

        bool gamepadPressed =
            Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame;

        if (keyboardPressed || gamepadPressed)
        {
            tutorialPanel.SetActive(false);
            tutorialActive = false;

            Time.timeScale = 1f;

            Destroy(gameObject);
        }
    }
}