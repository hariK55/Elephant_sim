using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HungerUI : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI textNo;
    #region singleton
    public static HungerUI instance { get;private set; }

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public float drainPerSecond;

    void Update()
    {
        textNo.text = slider.value.ToString("F0") + "%";
        slider.value -= drainPerSecond * Time.deltaTime;
        slider.value = Mathf.Clamp(slider.value, 0f, slider.maxValue);

        if (PlayerInteractor.Instance.HasObject() || Input.Instance.IsRunning())
        {
            drainPerSecond = 0.07f;
        }
       
    }
   
    private void Start()
    {
        slider.value = 30;
    }
    public void AddFood(int value)
    {
        slider.value += value;
    }
}
