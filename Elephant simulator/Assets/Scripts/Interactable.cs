using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, Iinteractable
{
   public static Interactable Instance { get; private set; }

    private Outline outline;
    [SerializeField]private bool isEnable =false;
    [SerializeField] private bool isEatable = false;
    [SerializeField] private int EatValue;
    private string displayStr = "Pick";
    

    //[SerializeField] private UnityEvent Oninteract;

    string Iinteractable.Display => displayStr;


    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 10f;
        outline.enabled = false;

        Instance = this;
    }
    public void Interact()
    {
        ElephantAnimation.Instance.pick();
       // Oninteract?.Invoke();
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

    public void Enable(bool enable)
    {
        isEnable = enable;
    }

    public void setEatable(bool eatable)
    {
        isEatable = eatable;
    }
    public bool IsEatable()
    {
        return isEatable;
    }

    public void SetEatValue(int value)
    {
        EatValue = value;
    }
    public int GetEatVAlue()
    {
        return EatValue;
    }
}
