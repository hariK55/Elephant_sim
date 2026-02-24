using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HungerUI : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI textNo;

    private bool isfade=false;
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
        slider.value = 1f;

    }
    public float drainPerSecond;

    void Update()
    {
        if(Input.Instance.caught)
        {
            return;
        }

        textNo.text = slider.value.ToString("F0") + "%";
        slider.value -= drainPerSecond * Time.deltaTime;
        slider.value = Mathf.Clamp(slider.value, 0f, slider.maxValue);

        valueBranches();


        if (slider.value >= 100f)
        {
            GameManager.Instance.WinGame();
        }
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
       
          

       /* if (slider.value < 10f)
        {
            FearMeter.Instance.LowFood(slider.value);
        }
         else if(slider.value > 10f)
        {
            FearMeter.Instance.ResetEffects();
        }*/
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
