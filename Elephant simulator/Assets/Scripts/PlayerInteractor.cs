using UnityEngine;
using UnityEngine.Windows;


public interface Iinteractable
{
    void Interact();
    void OnFocusGained();
    void OnFocusLost();

    bool CanInteract();

    string Display { get; }

    Transform transform { get; }
}

public class PlayerInteractor : MonoBehaviour
{
    private GameObject heldObject;
    [SerializeField] private Transform holdPoint;

    [SerializeField] private float radius = 2f;

    [SerializeField] private LayerMask interactableLayers;

    public static PlayerInteractor Instance { get; private set; }

    [SerializeField]
    private InteractPrompt prompt;

    private Collider[] buffer = new Collider[32];

    private Iinteractable focused;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Input.Instance.OnInteractPressed += Instance_OnInteractPressed;
        
    }
   

    private void Instance_OnInteractPressed(object sender, System.EventArgs e)
    {
        // CASE 1: Already holding something → DROP
        if (heldObject != null)
        {
            DropObject();
            ElephantAnimation.Instance.dropAnim();
            return;
        }

        // CASE 2: Not holding & focused object exists → PICK
        if (focused != null && focused.CanInteract())
        {
            focused.Interact();

            heldObject = ((MonoBehaviour)focused).gameObject;
            PickObject(heldObject);

        }
    }

    private void Update()
    {
        Iinteractable nearest = FindNearestInteractable();
        
        UpdateFocus(nearest);
        
       /* if(focused!=null && )
        {
            if (focused.CanInteract()) focused.Interact();
           
        }
       */
    }
    private Iinteractable FindNearestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, buffer, interactableLayers,QueryTriggerInteraction.Collide);
        
        Iinteractable nearest = null;

        float bestDistSq = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider collider = buffer[i];
            if (collider == null) continue;

            Iinteractable interactable = collider.GetComponentInParent<Iinteractable>();

            if (interactable == null) continue;

            if (!interactable.CanInteract()) continue;

            float distSq = (collider.transform.position - transform.position).sqrMagnitude;

            if(distSq<bestDistSq)
            {
                bestDistSq = distSq;
                nearest = interactable;
            }

        }
        return nearest;
    }

    private void UpdateFocus(Iinteractable nearest)
    {
        if (ReferenceEquals(focused, nearest)) return;

        focused?.OnFocusLost();

        focused = nearest;

        if (focused != null)
        {
            focused?.OnFocusGained();
            prompt.Show(focused);

        }
        else
        {
            prompt.Hide();
        }
        
    }

    public void PickObject(GameObject obj)
    {
        
        //heldObject = obj;

        // Disable physics while holding
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Attach object to hand/hold point
        obj.transform.SetParent(holdPoint, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    public void DropObject()
    {
        if (heldObject == null) return;
        // Detach
        heldObject.transform.SetParent(null);

        // Re-enable physics
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        heldObject = null;
    }
    public bool HasObject()
    {
        return heldObject != null;
    }
}
