using Unity.Cinemachine;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    bool isSliding;
    private Rigidbody rb;

    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float shakeForce = 0.08f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Footstep()
    {
     
        SoundManager.Instance.PlaySfx(Sound.footstep, 0.6f);
    }

    public void SlideFx()
    {
        SoundManager.Instance.PlaySfx(Sound.slide, 1f);
        isSliding = true;

    }

    public void shake()
    {
        // 🎥 CAMERA SHAKE
        if (Input.Instance.IsRunning() && Input.Instance.IsWalking())
            shakeForce = 0.17f;
        else shakeForce = 0.1f;

        if (impulseSource != null)
            impulseSource.GenerateImpulse(shakeForce);

    }

    private void Update()
    {
        if (Input.Instance.isSlidingDownhill &&Input.Instance.IsRunning())
        {
                if(!isSliding)
                {
                    SoundManager.Instance.PlaySfx(Sound.slide, 0.7f);
                    isSliding = true;
                }
            
        }
        if (!Input.Instance.isSlidingDownhill)
        {
            if (isSliding)
            {
                SoundManager.Instance.StopSound();
                isSliding = false;
            }
        }
       
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("vehicle"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // If surface normal points upward too much, block movement
                if (contact.normal.y < 0.5f)
                {
                    Vector3 horizontalVelocity = rb.linearVelocity;
                    horizontalVelocity.y = 0;
                    rb.linearVelocity = horizontalVelocity;
                }
            }
        }
    }
}
