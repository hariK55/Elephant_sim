using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text messageText;

    [Header("Animation")]
    [SerializeField] private float visibleTime = 0.5f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float moveDistance = 50f;

    private Vector3 originalPosition;

    private Queue<string> messageQueue = new();
    private bool isShowing;

    private void Awake()
    {
        Instance = this;

        originalPosition = messageText.rectTransform.localPosition;

        Color c = messageText.color;
        c.a = 0f;
        messageText.color = c;
    }

    public void ShowMessage(string message)
    {
        messageQueue.Enqueue(message);

        if (!isShowing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        while (messageQueue.Count > 0)
        {
            yield return StartCoroutine(
                AnimateMessage(messageQueue.Dequeue()));
        }

        isShowing = false;
    }

    private IEnumerator AnimateMessage(string message)
    {
        messageText.text = message;

        RectTransform rect = messageText.rectTransform;
        rect.localPosition = originalPosition;

        Color color = messageText.color;
        color.a = 1f;
        messageText.color = color;

        yield return new WaitForSeconds(visibleTime);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;

            color.a = Mathf.Lerp(1f, 0f, t);
            messageText.color = color;

            rect.localPosition =
                originalPosition +
                Vector3.up * Mathf.Lerp(0f, moveDistance, t);

            yield return null;
        }

        color.a = 0f;
        messageText.color = color;

        rect.localPosition = originalPosition;
    }
}