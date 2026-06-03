using UnityEngine;
using UnityEngine.UI;

public class EatUI : MonoBehaviour
{
    [SerializeField] private float holdDuration = 3.5f;
    [SerializeField] private Slider holdSlider;

    private float holdTimer = 0f;
    private bool isHolding = false;

    public static EatUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        holdSlider.minValue = 0f;
        holdSlider.maxValue = 1f;
        holdSlider.value = 0f;

        // Hide initially
        holdSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isHolding && PlayerInteractor.Instance.isEatable() && !Input.Instance.caught)
        {
            holdTimer += Time.deltaTime;

            holdSlider.value = holdTimer/holdDuration;

            if (holdTimer >= holdDuration)
            {
                ElephantAnimation.Instance.eatAnim(true);
                PlayerInteractor.Instance.OnEat();
                SoundManager.Instance.StopSound();

                ResetHold();
            }
        }
    }

    private void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;

        holdSlider.value = 0f;
        holdSlider.gameObject.SetActive(false); // Hide slider
    }

    public void OnHold()
    {
        if (PlayerInteractor.Instance.isEatable() && !Input.Instance.caught)
        {
            isHolding = true;

            holdSlider.gameObject.SetActive(true); // Show slider
           // holdSlider.value = 0f;

            SoundManager.Instance.PlaySfx(Sound.eatCane, 0.5f);
        }
    }

    public void OnHoldCanceled()
    {
        ResetHold();
        SoundManager.Instance.StopSound();
    }
}