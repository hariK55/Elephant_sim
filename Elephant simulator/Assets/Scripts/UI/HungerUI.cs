using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HungerUI : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI textNo;
    [SerializeField] private Image fillImage;

    //private bool isfade=false;
    private bool isdown=false;
    #region singleton
    public static HungerUI instance { get;private set; }

    private void Awake()
    {
        instance = this;
    }


    #endregion
    private void Start()
    {
        slider.value = 20f;

    }
    public float drainPerSecond;

    void Update()
    {
        if(Input.Instance.caught)
        {
            return;
        }
        if (slider.value >= 100f)
        {
            GameManager.Instance.WinGame();
        }

        textNo.text = slider.value.ToString("F0") + "%";
        slider.value -= drainPerSecond * Time.deltaTime;
        slider.value = Mathf.Clamp(slider.value, 0f, slider.maxValue);

        valueBranches();


       
    }
     private void valueBranches()
    {
        if (PlayerInteractor.Instance.HasObject() || Input.Instance.IsRunning())
        {
            drainPerSecond = 0.07f;
        }

        if (slider.value <= 0f && !isdown)
        {
            ElephantAnimation.Instance.Sleep();
            isdown = true;
            GameManager.Instance.LoseGame(5f,"you're Unconsiouss");
           
        }
        if (slider.value < 50f)
        {
            fillImage.color = Color.yellow;
            PlayerStamina.Instance.staminaDrainRate=6.5f;
            PlayerStamina.Instance.staminaRegenRate=5.5f;
        }
       else  if (slider.value < 15f)
        {
            fillImage.color = Color.red;
            PlayerStamina.Instance.staminaDrainRate = 6.7f;
            PlayerStamina.Instance.staminaRegenRate = 5.3f;
        }
        else if (slider.value >= 50f)
        {
            fillImage.color = Color.green;
            PlayerStamina.Instance.staminaDrainRate = 6f;
            PlayerStamina.Instance.staminaRegenRate = 6f;
        }

        if (slider.value < 5f)
        {
            Input.Instance.StopRunning();
        
            UnconsciousEffect.Instance.GoUnconscious();
              
        }


    }
  
    public void AddFood(int value)
    {
        slider.value += value;
    }
}
