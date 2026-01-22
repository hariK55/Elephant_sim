using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class FearMeter : MonoBehaviour
{

    public static FearMeter Instance { get; private set; }

    [Header("Fear Settings")]
    [SerializeField] private float fear = 0f;
    [SerializeField] private float maxFear = 100f;
    [SerializeField] private float fearIncreaseRate = 20f;
    [SerializeField] private float fearDecreaseRate = 10f;
  //  [SerializeField] private float drainRate=0.7f;
    [Header("References")]
    [SerializeField] private Slider fearSlider;
    [SerializeField] private Image fearOverlay;
   

    private bool nearFearSource;
    private float proximityMultiplier = 0.2f;


    [SerializeField] private Volume volume;

   // Bloom bloom;
    Vignette vignette;

   
    private void Awake()
    {
        Instance = this;
       // fearOverlay.gameObject.SetActive(true);
    }

    void Start()
    {
        //  volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
        PlayerInteractor.Instance.Eated += PlayerInteractor_Eated;
    }

    private void PlayerInteractor_Eated(object sender, System.EventArgs e)
    {
        fear -= 50;
    }

    void Update()
    {
        HandleFear();
        UpdateUI();
        ApplyEffects();
    }

    void HandleFear()
    {
       
        if (nearFearSource)
        {
            fear += fearIncreaseRate * proximityMultiplier * Time.deltaTime;

        }
        else
        {
            fear -= fearDecreaseRate * Time.deltaTime;
        }

        fear = Mathf.Clamp(fear, 0f, maxFear);

        vignette.intensity.value = fear / maxFear;
    }

    void UpdateUI()
    {
        fearSlider.value = fear;

      /*  Color overlayColor = fearOverlay.color;
        overlayColor.a = fear / maxFear;
        fearOverlay.color = overlayColor;*/
    }
   
    void ApplyEffects()
    {
        if (Input.Instance.caught) return;

        if (fear >= maxFear)
        {
            HungerUI.instance.drainPerSecond =0.8f;
            fearOverlay.color = Color.brown;

            AnxiousMusic();
        }

    
        else if(fear>50 && fear <maxFear)
        {
            HungerUI.instance.drainPerSecond = 0.5f;
            fearOverlay.color = Color.red;
            fearDecreaseRate = 2f;
            AnxiousMusic();

        }
        else if(fear<=50 && fear >0)
        {
            HungerUI.instance.drainPerSecond = 0.07f;
            fearOverlay.color = Color.darkOrange;
            fearDecreaseRate = 1f;

            AnxiousMusic();


        }
        else
        {

            HungerUI.instance.drainPerSecond = 0f;
            if (SoundManager.Instance.IsMusicPlaying(Music.Anxious))
                // SoundManager.Instance.FadeOut(10f);
                SoundManager.Instance.StopMusic();
        }

    }

    public void SetFearSource(bool state, float intensity)
    {
        nearFearSource = state;
        proximityMultiplier = intensity;
    }

    private void AnxiousMusic()
    {
        if (!SoundManager.Instance.IsMusicPlaying(Music.Anxious) && !SoundManager.Instance.IsMusicPlaying(Music.chase))
             SoundManager.Instance.PlayMusic(Music.Anxious,0.2f);
           // SoundManager.Instance.FadeIn(5f);
    }

    public void resetFear()
    {
        fear = 0f;
    }
}
