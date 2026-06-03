using UnityEngine;
using UnityEngine.UI;

public class EatUISlider : MonoBehaviour
{
    [SerializeField] private float holdDuration = 3.5f;
    [SerializeField] private Slider holdSlider;

    private float holdTimer = 0f;
    private bool isHolding = false;

    public static EatUISlider Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        holdSlider.minValue = 0f;
        holdSlider.maxValue = holdDuration;
        holdSlider.value = 0f;
    }

    private void Update()
    {
        if (isHolding && PlayerInteractor.Instance.isEatable() && !Input.Instance.caught)
        {
            holdTimer += Time.deltaTime;

            // Update slider
            holdSlider.value = holdTimer;

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
    }

    public void OnHold()
    {
        if (PlayerInteractor.Instance.isEatable() && !Input.Instance.caught)
        {
            isHolding = true;
            SoundManager.Instance.PlaySfx(Sound.eatCane, 0.5f);
        }
    }

    public void OnHoldCanceled()
    {
        ResetHold();
        SoundManager.Instance.StopSound();
    }
}