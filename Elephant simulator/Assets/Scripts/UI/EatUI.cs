using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EatUI: MonoBehaviour
{
    public float holdDuration;   // How long you have to hold down
    public Image fillCircle;

    private float holdTimer = 0f;
    private bool isHolding = false;

    public static EatUI Instance { get; private set; }

    private void Awake()
    {
        holdDuration = 5.5f;
        Instance = this;
    }

    void Update()
    {
        if (isHolding && PlayerInteractor.Instance.isEatable())
        {
           
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer /holdDuration ;
           
            if (holdTimer >= holdDuration)
            {
                ElephantAnimation.Instance.eatAnim(true);
                PlayerInteractor.Instance.OnEat();
               
                ResetHold();
            }
        }
    }

    

    private void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;
        fillCircle.fillAmount = 0f;
    }
    public void OnHold()
    {
        if (PlayerInteractor.Instance.HasObject())
        {
            isHolding = true;
        }
        else return;
    }
    public void OnHoldCanceled()
    {
        ResetHold();
       // ElephantAnimation.Instance.eatAnim(false);
    }
}
