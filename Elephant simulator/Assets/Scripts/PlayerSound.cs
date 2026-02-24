using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    bool isSliding;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Footstep()
    {
        SoundManager.Instance.PlaySfx(Sound.footstep, 0.2f);
    }

    public void SlideFx()
    {
        SoundManager.Instance.PlaySfx(Sound.slide, 0.3f);
        isSliding = true;

    }

    private void Update()
    {
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
