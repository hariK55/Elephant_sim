using TMPro;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);

    [SerializeField] protected string keyHint = "[E (or) y] to";

    private Camera cam;

    private Transform target;

    private Canvas canvas;

    private RectTransform canvasRect;

    private RectTransform labelRect;

    private void Awake()
    {
        cam = Camera.main;
        labelRect = label.rectTransform;
        canvas = label.GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        Hide();

    }

    void LateUpdate()
    {
        if (target == null)
        {
            
            return;
        }

        // Make sure label is visible
        if (!label.gameObject.activeSelf)
            label.gameObject.SetActive(true);

        // 1. World position + offset
        Vector3 worldPos = target.position + worldOffset;

        // 2. Convert world → screen
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        // If target behind camera, hide prompt
        if (screenPos.z < 0)
        {
            Hide();
            return;
        }

        // 3. Pick UI camera if needed
        Camera uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // 4. Convert screen → canvas local position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                uiCam,
                out Vector2 localPoint))
        {
            labelRect.anchoredPosition = localPoint;
        }
    }


    public void Show(Iinteractable interactable)
    {
        if (interactable == null)
        {
            Hide();
            return;
        }
        target = interactable.transform;
        label.text = $"{keyHint}{interactable.Display}";
        label.gameObject.SetActive(true);
    }
    public void Hide()
    {
        label.gameObject.SetActive(false);
        target = null;
    }




}
