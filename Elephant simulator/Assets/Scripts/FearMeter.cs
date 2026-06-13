using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class FearMeter : MonoBehaviour
{

    [SerializeField]
    private string tutorialMessage = "";

    [SerializeField]
    private TMP_Text tutorialText;
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
    private ColorAdjustments colorAdjustments;
    private bool firstText = false;

    private HungerUI hungerUI;
    private SoundManager soundManager;

    private enum FearState
    {
        None,
        Low,
        Medium,
        High
    }

    private FearState currentState;
    private FearState previousState;

    private void Awake()
    {
        Instance = this;
       // fearOverlay.gameObject.SetActive(true);
    }

    void Start()
    {
        hungerUI = HungerUI.instance;
        soundManager = SoundManager.Instance;
        //  volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);
        PlayerInteractor.Instance.Eated += PlayerInteractor_Eated;
    }

    private void PlayerInteractor_Eated(object sender, System.EventArgs e)
    {
        if(fear>0)
        {
            fear -= 50;
            NotificationUI.Instance.ShowMessage("Anxiety Reduced");
        }
        
    }

    void Update()
    {
        HandleFear();
        UpdateUI();
        UpdateFearState();


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

        float fearPercent = fear / maxFear;
        vignette.intensity.value = fearPercent * 0.75f;
    }

    void UpdateUI()
    {
        if (fearSlider.value != fear)
            fearSlider.value = fear;
    }

    private void UpdateFearState()
    {
        if (fear >= maxFear)
            currentState = FearState.High;
        else if (fear > 50f)
            currentState = FearState.Medium;
        else if (fear > 0f)
            currentState = FearState.Low;
        else
            currentState = FearState.None;

        if (currentState == previousState)
            return;

        previousState = currentState;

        switch (currentState)
        {
            case FearState.High:
                HungerUI.instance.drainPerSecond = 0.5f;
                fearOverlay.color = Color.brown;
                AnxiousMusic();
                break;

            case FearState.Medium:
                HungerUI.instance.drainPerSecond = 0.3f;
                fearOverlay.color = Color.red;
                fearDecreaseRate = 2f;
                AnxiousMusic();
                break;

            case FearState.Low:
                HungerUI.instance.drainPerSecond = 0.07f;
                fearOverlay.color = Color.yellow;
                fearDecreaseRate = 1f;
                AnxiousMusic();

                if (!EnemyAI.instance.CanSeePlayer())
                    tutorialText.text = tutorialMessage;

                firstText = true;
                break;

            case FearState.None:
                HungerUI.instance.drainPerSecond = 0.04f;

                if (SoundManager.Instance.IsMusicPlaying(Music.Anxious))
                    SoundManager.Instance.StopMusic();

                if (firstText)
                    tutorialText.text = "";

                break;
        }
    }
    void ApplyEffects()
    {
        if (Input.Instance.caught) return;

        if (fear >= maxFear)
        {
            if(hungerUI.drainPerSecond != 0.5f)
            {
                hungerUI.drainPerSecond = 0.5f;
                fearOverlay.color = Color.brown;

                AnxiousMusic();
            }
           
        }

    
        else if(fear>50 && fear <maxFear)
        {
            if(hungerUI.drainPerSecond != 0.3f)
            {
                hungerUI.drainPerSecond = 0.3f;
                fearOverlay.color = Color.red;
                fearDecreaseRate = 2f;
                AnxiousMusic();
            }
           

        }
        else if(fear<=50 && fear >0)
        {
            if(hungerUI.drainPerSecond != 0.07f)
            {
                hungerUI.drainPerSecond = 0.07f;
                fearOverlay.color = Color.darkOrange;
                fearDecreaseRate = 1f;

                AnxiousMusic();
                if (tutorialText.text != tutorialMessage)
                {
                    if (!EnemyAI.instance.CanSeePlayer())
                        tutorialText.text = tutorialMessage;
                }


                firstText = true;
            }
          

        }
        else
        {
            if(hungerUI.drainPerSecond != 0.04f)
            {
                hungerUI.drainPerSecond = 0.04f;
                if (soundManager.IsMusicPlaying(Music.Anxious))

                    soundManager.StopMusic();

                if (firstText)
                    tutorialText.text = "";
            }
              
        }

    }

    public void SetFearSource(bool state, float intensity)
    {
        nearFearSource = state;
        proximityMultiplier = intensity;
    }

    private void AnxiousMusic()
    {
        if (!soundManager.IsMusicPlaying(Music.Anxious) && !soundManager.IsMusicPlaying(Music.chase))
             soundManager.PlayMusic(Music.Anxious,0.4f);
           // soundManager.FadeIn(5f);
    }

    public void resetFear()
    {
        fear -=70f;
        NotificationUI.Instance.ShowMessage("Anxiety Reduced");
    }
   
    public void LowFood(float hunger)
    {
      
        // Convert hunger (10 → 5) into (0 → 1)
        float t = Mathf.InverseLerp(10f, 5f, hunger);

        colorAdjustments.saturation.value =
      Mathf.Lerp(colorAdjustments.saturation.value,
                 Mathf.Lerp(0f, -100f, t),
                 Time.deltaTime * 5f);
       
    }
    public void ResetEffects()
    {
        colorAdjustments.saturation.value = 0f;
        
    }
}

