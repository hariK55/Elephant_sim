using UnityEngine;

public class CreditScroller : MonoBehaviour
{
    public float scrollSpeed = 50f;
    public float endY = 2450f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (rectTransform.anchoredPosition.y > endY)
        {
            // Credits finished
            Debug.Log("Credits Complete");
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -1000f);

        }
    }
}