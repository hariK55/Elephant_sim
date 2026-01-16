using UnityEngine;
using UnityEngine.Windows;

#region IInteractable
public interface Iinteractable
{
    void Interact();
    void OnFocusGained();
    void OnFocusLost();

    bool CanInteract();

    string Display { get; }

    Transform transform { get; }
}

# endregion

public class PlayerInteractor : MonoBehaviour
{
    private GameObject focusedObject;
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
        if (Input.Instance.caught) return;

        // CASE 1: Already holding something → DROP
        if (focusedObject != null)
        {
            DropObject();
            ElephantAnimation.Instance.dropAnim();
            return;
        }

        // CASE 2: Not holding & focused object exists → PICK
        if (focused != null && focused.CanInteract())
        {
            focused.Interact();
            focusedObject = ((MonoBehaviour)focused).gameObject;


            PickObject(focusedObject);

        }
    }

    private void Update()
    {
        Iinteractable nearest = FindNearestInteractable();
        
        UpdateFocus(nearest);

        
        // Stop animation if mash input stopped
        if (focused != null && focusedObject==null)
        {
           
        }

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

        //focusedObject = obj;
        ElephantAnimation.Instance.eatAnim(false);
        // Disable physics while holdin
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = true;
       
        // Attach object to hand/hold point
        obj.transform.SetParent(holdPoint, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    public void DropObject()
    {
        if (focusedObject == null) return;
        // Detach
        focusedObject.transform.SetParent(null);
        focusedObject.GetComponent<Collider>().enabled = true;
        // Re-enable physics
        Rigidbody rb = focusedObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        focusedObject = null;
    }
    public bool HasObject()
    {
        return focusedObject != null;
    }
    public bool isEatable()
    {
        if (focusedObject== null) return false;
        return focusedObject.GetComponent<Interactable>().IsEatable();
    }
    public void OnEat()
    {
        EnemySoundSystem.EmitSound(transform.position, 15f);
        HungerUI.instance.AddFood(focusedObject.GetComponent<Interactable>().GetEatVAlue());
        UpdateFocus(null);
        focusedObject.SetActive(false);
        focusedObject = null;
    }

   
}
