using UnityEngine;

public class PushableTree : MonoBehaviour
{
    public Rigidbody rb;
    public float pushForce = 8f;
    public float fallTorque = 5f;

    private bool pushed = false;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = true; // stays still until pushed
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

        pushed = true;
        rb.isKinematic = false;

        // Push away from player
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);

        // Add torque so it topples naturally
        rb.AddTorque(Vector3.Cross(Vector3.up, pushDirection) * fallTorque, ForceMode.Impulse);
    }
}
