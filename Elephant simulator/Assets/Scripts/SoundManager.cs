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
    footvibe,
    pickCane,
    kumkiFootstep,
    thud,
    pushGrowl,
    growl,
    surrender,
}

public enum Music
{
    mainMenu,
    chase,
    Anxious,
    energy,
    Victory,
    pauseFX,
    loseMusic,
    bgMusic
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
      //  DontDestroyOnLoad(Instance);
    }


    private void Update()
    {
        if (!IsMusicPlaying(Music.bgMusic) && !musicSource.isPlaying)
            SoundManager.Instance.PlayMusic(Music.bgMusic, 0.2f);

    }
    public void PlaySfx(Sound sound,float volume)
    {
        if(sfxSource.clip==audioClips[(int)sound] && sfxSource.isPlaying)
            return;

        sfxSource.PlayOneShot(audioClips[(int)sound],1f);
    }

  public void PlayMusic(Music music,float volume)
  {
        StopMusic();
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
