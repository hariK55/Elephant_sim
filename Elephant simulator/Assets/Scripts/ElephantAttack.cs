using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class ElephantAttack : MonoBehaviour
{

    public static ElephantAttack Instance { get; private set; }

    [Header("Attack Settings")]
    public float minForce = 5f;
    public float maxForce = 25f;
    public float maxChargeTime = 2f;
    public float flipTorque = 15f;
    public float attackRange = 2f;

    InputSystem inputActions;
    private float holdTime;
    private bool isCharging;
   

    private void Awake()
    {
        inputActions = new InputSystem();
        Instance = this;
    }

    private void OnEnable()
    {
        inputActions.Player.Attack.started += OnAttackStarted;
        inputActions.Player.Attack.canceled += OnAttackCanceled;
       

        inputActions.Enable();
    }



    private void OnDisable()
    {
        inputActions.Player.Attack.started -= OnAttackStarted;
        inputActions.Player.Attack.canceled -= OnAttackCanceled;
        inputActions.Disable();
    }
    
    private void Update()
    {
        if (isCharging && !Input.Instance.caught)
        {
            holdTime += Time.deltaTime;
            holdTime = Mathf.Clamp(holdTime, 0f, maxChargeTime);
        }
       
    }

    private void OnAttackStarted(InputAction.CallbackContext ctx)
    {
        if (Input.Instance.caught) return;
        //  if (holding) return;   // prevent re-entry

        isCharging = true;
        holdTime = 0f;

        ElephantAnimation.Instance.HoldAttack(true);
       // holding = true;
        
    }



    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
        if (Input.Instance.caught) return;
        if (!isCharging) return;


        isCharging = false;

        ElephantAnimation.Instance.Attack();
        PerformAttack();
    }
   

    void PerformAttack()
    {
        if (Input.Instance.caught) return;

        SoundManager.Instance.PlaySfx(Sound.attack, 1f);

        float chargePercent = holdTime / maxChargeTime;
        float force = Mathf.Lerp(minForce, maxForce, chargePercent);

        float radius = 0.5f;   // Increase for more forgiveness
        RaycastHit hit;
        if (Physics.SphereCast(
            transform.position + Vector3.up * 1f,
            radius,
            transform.forward,
            out hit,
            attackRange)) 
        { 
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {

                // Apply force
                rb.AddForce(transform.forward * force, ForceMode.Impulse);
                Debug.Log("hit");
                // Flip on charged attack
                if (chargePercent > 0.6f)
                {
                    if (rb.gameObject.CompareTag("vehicle"))
                    {
                        SoundManager.Instance.PlaySfx(Sound.heavyHit, 0.7f);
                        rb.gameObject.GetComponent<FearSource>().DisableFearSource();
                        FearMeter.Instance.resetFear();
                    }
                       
                    rb.AddTorque(transform.right * flipTorque, ForceMode.Impulse);
                }
                else
                {
                    if(rb.gameObject.CompareTag("vehicle"))
                    {
                        SoundManager.Instance.PlaySfx(Sound.hitCar, 0.5f);
                    }
                   
                }
            }
            else
            {
                
            }
        }
    }
    public bool IsCharging()
    {
        return isCharging;
    }
   
}
