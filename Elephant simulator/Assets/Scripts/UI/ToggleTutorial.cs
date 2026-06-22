using TMPro;
using UnityEngine;

public class ToggleTutorial : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;

    private void Start()
    {
        UpdateText();
    }

    public void ToggleTutorials()
    {
        bool enabled =
            PlayerPrefs.GetInt("TutorialsEnabled", 1) == 1;

        PlayerPrefs.SetInt(
            "TutorialsEnabled",
            enabled ? 0 : 1);

        PlayerPrefs.Save();

        UpdateText();
    }

    private void UpdateText()
    {
        bool enabled =
            PlayerPrefs.GetInt("TutorialsEnabled", 1) == 1;

        buttonText.text =
            "Tutorials: " + (enabled ? "ON" : "OFF");
    }
}