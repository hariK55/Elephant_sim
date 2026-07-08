using UnityEngine;

public class KumkiAlertIndicator : MonoBehaviour
{
    public Camera playerCamera;
    public Transform kumkiHead;
    public RectTransform alertUI;

    public float headOffset = 1.5f;
    public float edgePadding = 60f;

    void LateUpdate()
    {
        if (kumkiHead == null) return;

        Vector3 worldPos = kumkiHead.position + Vector3.up * headOffset;
        Vector3 screenPos = playerCamera.WorldToScreenPoint(worldPos);

        bool visible =
            screenPos.z > 0 &&
            screenPos.x >= 0 &&
            screenPos.x <= Screen.width &&
            screenPos.y >= 0 &&
            screenPos.y <= Screen.height;

        if (visible)
        {
            // Place above Kumki
            alertUI.position = screenPos;
        }
        else
        {
            // If behind camera, mirror it
            if (screenPos.z < 0)
                screenPos *= -1;

            Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 dir = ((Vector2)screenPos - center).normalized;

            float x = Mathf.Clamp(
                center.x + dir.x * (Screen.width / 2f - edgePadding),
                edgePadding,
                Screen.width - edgePadding);

            float y = Mathf.Clamp(
                center.y + dir.y * (Screen.height / 2f - edgePadding),
                edgePadding,
                Screen.height - edgePadding);

            alertUI.position = new Vector2(x, y);
        }
    }
}