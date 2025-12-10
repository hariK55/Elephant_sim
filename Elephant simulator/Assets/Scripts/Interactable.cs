using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, Iinteractable
{
   
    private Outline outline;
    private bool isEnable = true;

    private string displayStr = "Pick";

    [SerializeField] private UnityEvent Oninteract;

    string Iinteractable.Display => displayStr;

    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 10f;
        outline.enabled = false;
    }
    public void Interact()
    {
       
        Oninteract?.Invoke();
      //  Input.Instance.isInteract = false;
    }

    

    public void OnFocusGained()
    {
        outline.enabled = true;
    }

    public void OnFocusLost()
    {
        outline.enabled = false;
    }

    public bool CanInteract()
    {
        return isEnable;
    }

    
}
