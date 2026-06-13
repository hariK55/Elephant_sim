using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Button button;

    public TextMeshProUGUI buttonText;

    private bool tutorialEnabled = true;

    void Start()
    {
        button.onClick.AddListener(OnTutorialButtonClicked);
       
    }

    private void OnTutorialButtonClicked()
    {
        
        tutorialEnabled = !tutorialEnabled;

        buttonText.text = tutorialEnabled
            ? "Tutorial: ON"
            : "Tutorial: OFF";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
