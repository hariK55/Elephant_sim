using UnityEngine;

public class PushableTree : MonoBehaviour
{
    [Header("Physics")]
    public Rigidbody rb;
    public float pushForce = 8f;
    public float fallTorque = 5f;

    [Header("Sound")]
    public float treeFallSound = 500f;

    private bool fallen = false;

    private Interactable interactable;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        interactable = GetComponent<Interactable>();

        rb.isKinematic = true; // Tree stays still until fallen
    }

    void Start()
    {
        if (interactable != null)
        {
            interactable.setEatable(false);
            interactable.Enable(false);
        }
    }

    // ✅ Used by RapidPressMechanic
    public bool IsFallen()
    {
        return fallen;
    }

    // ✅ Called once when mash progress completes
    public void FallDown(Vector3 pushDirection)
    {
        if (fallen) return;

        fallen = true;

        // Enable physics
        rb.isKinematic = false;

        // Enable interaction after fall
        if (interactable != null)
            interactable.Enable(true);

        // 🎵 Sounds
        SoundManager.Instance.PlaySfx(Sound.TreeFall, 0.7f);
        SoundManager.Instance.PlaySfx(Sound.pushGrowl, 0.5f);

        EnemySoundSystem.EmitSound(transform.position, treeFallSound);

        // 💥 Apply forward impulse
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);

        // 🌳 Natural toppling torque
        Vector3 torqueDir = Vector3.Cross(Vector3.up, pushDirection);
        rb.AddTorque(torqueDir * fallTorque, ForceMode.Impulse);

        //avoid weird bounce and clipping through ground
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}