using UnityEngine;
using UnityEngine.EventSystems;

public class MobileLookArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static Vector2 LookDelta;

    private Vector2 lastPosition;
    private bool dragging;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        lastPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        Vector2 delta = eventData.position - lastPosition;
        LookDelta = delta;
        lastPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        LookDelta = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!dragging)
            LookDelta = Vector2.zero;
    }
}