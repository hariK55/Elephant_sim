using System;
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

    [SerializeField] private float radius = 2.5f;

    [SerializeField] private LayerMask interactableLayers;

    public static PlayerInteractor Instance { get; private set; }

    public event EventHandler Eated;

    [SerializeField]
    private InteractPrompt prompt;

    private Collider[] buffer = new Collider[32];

    private Iinteractable focused;

    private string displayStr;

    private float scanTimer;
    private const float ScanInterval = 0.1f;

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
            focused.Interact();//pick
            focusedObject = ((MonoBehaviour)focused).gameObject;


            PickObject(focusedObject);

        }
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;

        if (scanTimer <= 0f)
        {
            scanTimer = ScanInterval;

            Iinteractable nearest = FindNearestInteractable();
            UpdateFocus(nearest);
        }

        if (holdingTree)
            Input.Instance.StopRunning();
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
            prompt.showTake();

        }
        else
        {
            if(!HasObject())
            {
                 prompt.Hide();
            }
           
        }
        
    }
    public static event Action OnpickEatTutorial;

    private Collider heldCollider;
    private Rigidbody heldRb;
    private Interactable heldInteractable;
    public void PickObject(GameObject obj)
    {

        heldInteractable = obj.GetComponent<Interactable>();
        ElephantAnimation.Instance.eatAnim(false);
        holdingTree = obj.GetComponent<PushableTree>() != null;

        SoundManager.Instance.PlaySfx(Sound.thud, 0.5f);

        // Disable physics while holdin
        heldCollider = obj.GetComponent<Collider>();
        if (heldCollider != null)
            heldCollider.enabled = false;

        heldRb = obj.GetComponent<Rigidbody>();
        heldRb.isKinematic = true;
       
        // Attach object to hand/hold point
        obj.transform.SetParent(holdPoint, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        
        if (obj.GetComponent<coconut>())
        {
            obj.transform.localPosition = new Vector3(-5.34f, -4.05f, 0f);
        }
        else if (obj.GetComponent<SugarCane>())
        {
            obj.transform.localPosition = new Vector3(0f, -1.63f, 0f);
        }
        else if (obj.CompareTag("banana"))
        {
            obj.transform.localPosition = new Vector3(-0.31f, -0.219f, 0.2f);
            obj.transform.localRotation = new Quaternion(0.620607018f, 0.0860012397f, -0.0728725418f, 0.775977075f);
        }
        if (isEatable())
        {
            OnpickEatTutorial?.Invoke();
            prompt.Show(focused);
            prompt.showEat();
        }
    }

    public void DropObject()
    {
        holdingTree = false;
        if  (focusedObject!=null &&  focusedObject.GetComponent<PushableTree>())
            SoundManager.Instance.PlaySfx(Sound.thud, 0.5f);

        if (focusedObject != null && focusedObject.CompareTag("eatable"))
            SoundManager.Instance.PlaySfx(Sound.drop, 1f);


        if (focusedObject == null) return;
        // Detach
       

        focusedObject.transform.SetParent(null);
        heldCollider.enabled = true;
        heldCollider.isTrigger = false;

        // Re-enable physics
        heldRb.isKinematic = false;

        focusedObject = null;
        heldCollider = null;
        heldRb = null;
        heldInteractable = null;
    }
    public bool HasObject()
    {
        return focusedObject != null;
    }
    public bool isEatable()
    {
        if (focusedObject== null) return false;
       return heldInteractable != null &&
       heldInteractable.IsEatable();
    }
    public void OnEat()
    {
        prompt.Hide();
        SoundManager.Instance.PlaySfx(Sound.eatCane, 0.5f);
        SoundManager.Instance.PlayMusic(Music.energy, 0.7f);

        int eatValue = heldInteractable.GetEatVAlue();
        HungerUI.instance.AddFood(eatValue);
        NotificationUI.Instance.ShowMessage("+"+eatValue+" Energy");
        Eated?.Invoke(this, EventArgs.Empty);

        UpdateFocus(null);
        focusedObject.SetActive(false);
        focusedObject = null;
       

       
    }
    private bool holdingTree;
    public bool HasTree()
    {
        return holdingTree;
    }
}
