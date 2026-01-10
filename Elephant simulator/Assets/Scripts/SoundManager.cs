using Unity.VisualScripting;
using UnityEngine;


public enum Sound
{
    hitCar,
    eatCane,
    footstep,
    slide,
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

   [SerializeField] private AudioSource audioSource;
  


    

    [SerializeField]private AudioClip[] audioClips;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
       
    }

    public void PlayOneShot(Sound sound,float volume)
    {
        audioSource.PlayOneShot(audioClips[(int)sound],volume);
    }

  

    public void StopSound()
    {
        audioSource.Stop();
    }

    
}
