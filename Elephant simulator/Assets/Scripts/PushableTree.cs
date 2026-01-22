using UnityEngine;

public class PushableTree : MonoBehaviour
{
    public Rigidbody rb;
    public float pushForce = 8f;
    public float fallTorque = 5f;
    public float treeFallSound= 200f;
    private bool pushed = false;
    Interactable interactable;
    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        interactable= GetComponent<Interactable>();
        rb.isKinematic = true; // stays still until pushed
    }

    private void Start()
    {
       interactable.setEatable(false);
        interactable.Enable(false);
    }


    // Called continuously while mashing (small shake)
    public void PushTree(Vector3 pushDirection, float strength)
    {
        if (pushed) return;

        rb.isKinematic = false;
        rb.AddForce(pushDirection * strength, ForceMode.Force);
    }

    // Called once when progress is full
    public void FallDown(Vector3 pushDirection)
    {
        if (pushed) return;

        SoundManager.Instance.PlaySfx(Sound.TreeFall, 0.7f);
        pushed = true;
        rb.isKinematic = false;
        interactable.Enable(true);
        // Push away from player
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        EnemySoundSystem.EmitSound(transform.position, treeFallSound);
        // Add torque so it topples naturally
        rb.AddTorque(Vector3.Cross(Vector3.up, pushDirection) * fallTorque, ForceMode.Impulse);

        
    }
}
