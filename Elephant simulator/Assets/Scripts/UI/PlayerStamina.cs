using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{

    public static PlayerStamina Instance { get; private set; }

    private bool canRun;


    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 8f;   // per second when running
    public float staminaRegenRate = 6f;   // per second when idle

    [Header("UI")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField]
    private Image fill;
    // higher = snappier, lower = smoother

    private float currentStamina;
    private float displayedStamina;        // what the UI shows

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        currentStamina = maxStamina;
        displayedStamina = maxStamina;

        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;

       
    }

    void Update()
    {
        if (Input.Instance.caught) return;

        if (Input.Instance.IsRunning() && currentStamina > 0 && Input.Instance.IsWalking())
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;

            if (currentStamina > 0 && currentStamina <= 30)
            {
              
                fill.color = Color.red;
            }
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;

            if (currentStamina > 0 && currentStamina <= 30)
            {
                canRun = false;
                fill.color = Color.red;
            }
            else canRun = true;
        }

        // Smoothly animate UI value toward actual stamina
        displayedStamina = Mathf.Lerp(displayedStamina, currentStamina, Time.deltaTime * smoothSpeed);
        staminaSlider.value = displayedStamina;

        if (currentStamina <= 0f)
        {
            canRun = false;
        }
        /* else
         {
             canRun = true;
         }*/
        if (currentStamina > 30) fill.color = Color.yellow;
    }


    public bool hasStamina()
    {
        return canRun;
    }
}