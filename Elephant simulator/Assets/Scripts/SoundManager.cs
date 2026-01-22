using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public enum Sound
{
    hitCar,
    eatCane,
    footstep,
    slide,
    TreeShake,
    TreeFall,
    heavyHit,
    drop,
    Trumpet,
    attack,
    pickCane,
    kumkiFootstep,
    thud,
}

public enum Music
{
    mainMenu,
    chase,
    Anxious,
    
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;


    [SerializeField] private AudioSource musicSource;




    [SerializeField]private AudioClip[] audioClips;

    [SerializeField] private AudioClip[] musicClips;

    private void Awake()
    {
      
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

   
    public void PlaySfx(Sound sound,float volume)
    {
        sfxSource.PlayOneShot(audioClips[(int)sound],volume);
    }

  public void PlayMusic(Music music,float volume)
  {

        if (!IsMusicPlaying(music))
        {
            musicSource.PlayOneShot(musicClips[(int)music], volume);

            musicSource.clip = musicClips[(int)music];
        } 
  }

    public void StopSound()
    {
        sfxSource.Stop();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void FadeOut(float fadeDuration)
    {
        StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // reset for next play
    }

    public bool IsMusicPlaying(Music music)
    {
        return musicSource.isPlaying && musicSource.clip==musicClips[(int)music];
    }

    
   

}
