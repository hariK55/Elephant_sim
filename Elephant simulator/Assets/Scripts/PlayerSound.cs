using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    bool isSliding;
   public void Footstep()
    {
        SoundManager.instance.PlayOneShot(Sound.footstep, 0.3f);
    }

    public void SlideFx()
    {
        SoundManager.instance.PlayOneShot(Sound.slide, 0.3f);
        isSliding = true;

    }

    private void Update()
    {
        if (!Input.Instance.isSlidingDownhill)
        {
            if (isSliding)
            {
                SoundManager.instance.StopSound();
                isSliding = false;
            }
        }
    }


}
