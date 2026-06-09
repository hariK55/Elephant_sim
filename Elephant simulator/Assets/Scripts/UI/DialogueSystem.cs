using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private GameObject firstButton;
    [Header("UI")]
    public TMP_Text dialogueText;
    public Button continueButton;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] dialogues;

    [Header("Typewriter")]
    public float typingSpeed = 0.03f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound;

    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
       

        continueButton.onClick.AddListener(OnContinueClicked);
        StartDialogue();
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void StartDialogue()
    {
        Time.timeScale = 0f; // Pause game

        currentDialogueIndex = 0;
        typingCoroutine = StartCoroutine(TypeText(dialogues[currentDialogueIndex]));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        if (typingSound != null &&
               audioSource != null)
        {
            audioSource.PlayOneShot(typingSound, 0.3f);
        }

        foreach (char letter in text)
        {
            dialogueText.text += letter;

            yield return new WaitForSecondsRealtime(typingSpeed);
        }
       
        audioSource.Stop();

        isTyping = false;
    }

    public void OnContinueClicked()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);

            dialogueText.text = dialogues[currentDialogueIndex];
            isTyping = false;
            return;
        }

        currentDialogueIndex++;

        if (currentDialogueIndex >= dialogues.Length)
        {
            EndDialogue();
            return;
        }

        typingCoroutine =
            StartCoroutine(TypeText(dialogues[currentDialogueIndex]));
    }

    void EndDialogue()
    {
        gameObject.SetActive(false);
        audioSource.Stop();
        EventSystem.current.SetSelectedGameObject(null);
        // Optional:
        Time.timeScale = 1f;
        // Enable player movement here
    }
}
