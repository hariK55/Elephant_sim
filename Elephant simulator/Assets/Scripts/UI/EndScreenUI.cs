using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EndScreenUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI resultText;
    public float fadeSpeed = 2f;

    Image panelImage;
    Color color;

    void Start()
    {
        panelImage = panel.GetComponent<Image>();
        color = panelImage.color;
        color.a = 0;
        panelImage.color = color;
        panel.SetActive(false);
    }

    public void Show(string message)
    {
        resultText.text = message;
        panel.SetActive(true);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Time.timeScale = 0f;

        while (color.a < 0.8f)
        {
            color.a += fadeSpeed * Time.unscaledDeltaTime;
            panelImage.color = color;
            yield return null;
        }
    }
}
