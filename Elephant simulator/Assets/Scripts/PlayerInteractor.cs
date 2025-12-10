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
    [SerializeField] private float radius = 2f;

    [SerializeField] private LayerMask interactableLayers;

    [SerializeField]
    private InteractPrompt prompt;

    private Collider[] buffer = new Collider[32];

    private Iinteractable focused;

    private void Start()
    {
        Input.Instance.OnInteractPressed += Instance_OnInteractPressed;
    }
   

    private void Instance_OnInteractPressed(object sender, System.EventArgs e)
    {
        if (focused != null)
        {
            if (focused.CanInteract()) focused.Interact();

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

        for(int i = 0; i < count; i++)
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
}
