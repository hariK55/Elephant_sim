using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnconsciousEffect : MonoBehaviour
{
    public static UnconsciousEffect Instance { get; private set; }
    public Image blackImage;
    public float fadeSpeed = 0.05f;
    public float unconsciousTime = 4f;
    private bool isUnconscious = false;
    private void Awake()
    {
        Instance = this;
    }

    public void GoUnconscious()
    {
        if (!isUnconscious)
        {
            StartCoroutine(FadeRoutine());
            SoundManager.Instance.PlaySfx(Sound.pushGrowl,0.1f);
        }
       
    }

    IEnumerator FadeRoutine()
    {
        isUnconscious = true;
        // Fade to black
        while (blackImage.color.a < 1f)
        {
            float newAlpha = Mathf.MoveTowards(
                blackImage.color.a,
                1f,
                fadeSpeed * Time.deltaTime
            );

            blackImage.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        // Stay black
        yield return new WaitForSeconds(unconsciousTime);

        // Fade back to normal
        while (blackImage.color.a > 0f)
        {
            float newAlpha = Mathf.MoveTowards(
                blackImage.color.a,
                0f,
                fadeSpeed * Time.deltaTime
            );

            blackImage.color = new Color(0, 0, 0, newAlpha);
          
            yield return null;
        }
        isUnconscious = false;
    }

}