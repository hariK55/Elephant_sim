using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour, Iinteractable
{
   public static Interactable Instance { get; private set; }

    private Outline outline;
    [SerializeField]private bool isEnable =false;
    [SerializeField] private bool isEatable = false;
    [SerializeField] private int EatValue;
    private string displayStr;
    

    //[SerializeField] private UnityEvent Oninteract;
    

    [SerializeField] private InputActionReference eatAction;
    [SerializeField] private InputActionReference pickAction;


    string Iinteractable.Display => displayStr;

    private Renderer rend;


    private void Awake()
    {
       
        rend = GetComponentInChildren<Renderer>();

       

        PlayerInteractor.OnpickEatTutorial += PlayerInteractor_OnpickEatTutorial;
        /*outline = gameObject.AddComponent<Outline>();
       
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.cyan;
        outline.OutlineWidth = 10f;
        outline.enabled = false;*/

        Instance = this;
    }

    private void PlayerInteractor_OnpickEatTutorial()
    {
        displayStr = $"Hold {currentBindingEat} to Eat";
    }

    private void Start()
    {
        if ((gameObject.tag) != "Tree")
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", Color.yellow * 10f);
        }
    }

    private string lastBinding = "";
    private string lastBindingPick = "";
    string currentBindingPick="";
    string currentBindingEat="";
    void Update()
    {
        currentBindingEat =
            eatAction.action.GetBindingDisplayString();
         currentBindingPick =
           pickAction.action.GetBindingDisplayString();

        if (currentBindingEat != lastBinding)
        {
            lastBinding = currentBindingEat;
           
        }

        if (currentBindingPick != lastBindingPick)
        {
            lastBindingPick = currentBindingPick;
            displayStr = $"Press {currentBindingPick} to Pick";

        }
    }
    public void Interact()
    {
        ElephantAnimation.Instance.pick();
        displayStr = $"Press {currentBindingPick} to Pick";
    }

    

    public void OnFocusGained()
    {
        //outline.enabled = true;
        displayStr = $"Press {currentBindingPick} to Pick";

    }

    public void OnFocusLost()
    {
        rend.material.SetColor("_EmissionColor", Color.black);
        displayStr = $"Press {currentBindingPick} to Pick";
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
