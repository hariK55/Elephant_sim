using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleCrashSound : MonoBehaviour
{
    [Header("Impact Sound")]
    public AudioSource impactAudio;
    public float minImpactForce = 1;
    public float maxImpactForce = 20f;
    public float impactCooldown = 0.4f;

    [Header("Flip Sound")]
    public AudioSource flipAudio;
    public float flipThreshold = 0.3f;      // Dot threshold
    public float flipTimeRequired = 1.2f;   // Time before considered flipped

    private Rigidbody rb;
    private float lastImpactTime = 0f;
    private float flipTimer = 0f;
    private bool flipSoundPlayed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckFlip();
    }

    // ---------------- COLLISION IMPACT ----------------

    void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= minImpactForce &&
            Time.time - lastImpactTime > impactCooldown)
        {
            PlayImpactSound(impactForce);
            lastImpactTime = Time.time;
        }
    }

    void PlayImpactSound(float force)
    {
        if (impactAudio == null) return;

        float normalizedForce = Mathf.InverseLerp(minImpactForce, maxImpactForce, force);
        impactAudio.volume = Mathf.Clamp01(normalizedForce);
        //impactAudio.volume = 0.5f;
        impactAudio.Play();
    }

    // ---------------- FLIP DETECTION ----------------

    void CheckFlip()
    {
        float dot = Vector3.Dot(transform.up, Vector3.up);

        if (dot < flipThreshold)
        {
           
            PlayFlipSound();
            flipSoundPlayed = true;
        }
        else
        {
            
            flipSoundPlayed = false;
        }
    }

    void PlayFlipSound()
    {
        if (flipAudio != null)
        {
            flipAudio.Play();
        }
    }
}
