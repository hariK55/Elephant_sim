using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RapidPressMechanic : MonoBehaviour
{
    public static RapidPressMechanic Instance { get; private set; }

    [Header("Progress Settings")]
    public float progress = 0f;
    public float maxProgress = 100f;
    public float pressAmount = 10f;
    public float decayRate = 15f;

    [Header("Movement Check")]
    [Range(0f, 1f)]
    public float moveTowardThreshold = 0.2f;

    [Header("UI")]
    public Slider progressSlider;

    [Header("Vibration")]
    public float minVibration = 0.1f;
    public float maxVibration = 0.8f;

    private bool canPush = false;
    private bool completed = false;

    private PushableTree currentTree;
    private Rigidbody rb;
    private Gamepad gamepad;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        gamepad = Gamepad.current;

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopVibration();
    }

    void Update()
    {
        if (!canPush || completed) return;

        // Decay progress
        progress -= decayRate * Time.deltaTime;
        progress = Mathf.Clamp(progress, 0, maxProgress+1f);

        UpdateUI();
        UpdateVibration();
    }

    // 🔹 Called by Input (button mash)
    public void OnMash()
    {
        if (!canPush || completed) return;

        // ❌ BLOCK if player is NOT moving toward the tree
        if (!IsPushingTree())
            return;


        if (progressSlider != null)
            progressSlider.gameObject.SetActive(true);
        if(Input.Instance.IsRunning())
        {
            progress += pressAmount+1.2f;
        }
        else
        {
            progress += pressAmount;
        }
       
        progress = Mathf.Clamp(progress, 0, maxProgress+1f);

        if (progress >= maxProgress)
        {
            completed = true;
            currentTree.FallDown(GetPushDirection());
            OnCompleted();
        }
    }

    private void UpdateUI()
    {
        if (progressSlider == null) return;
        progressSlider.value = progress / maxProgress;
    }

    private void UpdateVibration()
    {
        if (gamepad == null) return;

        float normalized = progress / maxProgress;
        float strength = Mathf.Lerp(minVibration, maxVibration, normalized);
        gamepad.SetMotorSpeeds(strength, strength);
    }

    private void StopVibration()
    {
        if (gamepad == null) return;
        gamepad.SetMotorSpeeds(0f, 0f);
    }

    private void OnCompleted()
    {
        Debug.Log("Mash Completed!");

        StopVibration();

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(false);

        progress = 0f;
    }

    // 🔹 Direction away from player
    private Vector3 GetPushDirection()
    {
        Vector3 dir = currentTree.transform.position - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }
    private bool IsPushingTree()
    {
        if (currentTree == null) return false;
        Vector2 move = Input.Instance.GetMovementVector();
        if (move == Vector2.zero) return false;

        Vector3 pushDir = GetPushDirection();   // player → tree
        Vector3 forward = transform.forward;    // elephant facing direction

        forward.y = 0f;

        float dot = Vector3.Dot(forward.normalized, pushDir);

        // Player must be facing the tree
        return dot > 0.4f;
    }

    // 🔹 NEW: Movement toward tree check
    private bool IsMovingTowardTree()
    {
        if (rb == null || currentTree == null)
            return false;
        Vector2 move = Input.Instance.GetMovementVector();
        Vector3 velocity = new Vector3(move.x, 0, move.y);
        velocity.y = 0f;

        if (velocity.magnitude < 0.1f)
            return false; // standing still

        Vector3 moveDir = velocity.normalized;
        Vector3 pushDir = GetPushDirection();
        Debug.Log("pushdir:" + pushDir);
        float dot = Vector3.Dot(moveDir, pushDir);
        Debug.Log("dot:" + dot);
        return dot > moveTowardThreshold;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tree"))
        {
            currentTree = collision.gameObject.GetComponent<PushableTree>();
            canPush = true;
            progress = 0f;
            completed = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tree"))
        {
            canPush = false;
            currentTree = null;
            progress = 0f;

            if (progressSlider != null)
                progressSlider.gameObject.SetActive(false);

            StopVibration();
        }
    }
}
