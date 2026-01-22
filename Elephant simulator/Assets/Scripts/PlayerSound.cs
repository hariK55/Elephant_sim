using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    bool isSliding;
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


}
