using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleDragSoundSystem : MonoBehaviour
{
    [Header("Drag Sound")]
    public AudioSource dragAudio;
    public float minDragSpeed = 2f;
    public float maxDragSpeed = 15f;

    [Header("Ground Check")]
    public float groundCheckDistance = 1.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (dragAudio != null)
        {
            dragAudio.loop = true;
            dragAudio.playOnAwake = false;
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleDragSound();
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down,
                                     groundCheckDistance, groundLayer);
    }

    void HandleDragSound()
    {
        if (dragAudio == null) return;

        float speed = rb.linearVelocity.magnitude;

        if (isGrounded && speed > minDragSpeed)
        {
            if (!dragAudio.isPlaying)
                dragAudio.Play();

            // Volume based on speed
            float volume = Mathf.InverseLerp(minDragSpeed, maxDragSpeed, speed);
           // dragAudio.volume = Mathf.Clamp01(volume);
            dragAudio.volume = 0.5f; // Optional fixed volume, comment out if using speed-based volume
            // Optional pitch variation
            dragAudio.pitch = 0.8f + volume * 0.5f;
        }
        else
        {
            if (dragAudio.isPlaying)
                dragAudio.Stop();
        }
    }
}
