
using Unity.Cinemachine;
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

    [Header("Tree Detection")]
    public float detectionRadius = 1.2f;
    public float detectionDistance = 2f;
    public LayerMask treeLayer;

    [Header("Forward Push Requirement")]
    [Range(0f, 1f)]
    public float forwardDotThreshold = 0.6f;

    [Header("UI")]
    public Slider progressSlider;

    [Header("Camera Shake")]
    public CinemachineImpulseSource impulseSource;
    public float shakeForce = 0.2f;

    [Header("Vibration")]
    public float minVibration = 0.1f;
    public float maxVibration = 0.8f;

    private bool canPush = false;
    private bool completed = false;

    private PushableTree currentTree;
    private Gamepad gamepad;

    private float lastMashTime;
    public float mashTimeout = 0.25f;

    private float detectTimer;
    private const float DetectInterval = 0.1f;

    private bool sliderVisible;

    private ElephantAnimation elephantAnim;
    void Awake()
    {
        Instance = this;
        gamepad = Gamepad.current;

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(false);
    }
    private void Start()
    {
        elephantAnim = ElephantAnimation.Instance;
    }
    void OnDisable()
    {
        StopVibration();
    }

    void Update()
    {
        detectTimer -= Time.deltaTime;

        if (detectTimer <= 0f)
        {
            detectTimer = DetectInterval;
            DetectTree();
        }

        if (!canPush || completed) return;

        // Decay
        progress -= decayRate * Time.deltaTime;
        progress = Mathf.Clamp(progress, 0, maxProgress);

        UpdateUI();
        UpdateVibration();

        if (ElephantAnimation.Instance.getPushing())
        {
            if (Time.time - lastMashTime > mashTimeout)
                ElephantAnimation.Instance.PushAnim(false);
        }
    }

    private void SetSliderVisible(bool visible)
    {
        if (sliderVisible == visible) return;

        sliderVisible = visible;
        progressSlider.gameObject.SetActive(visible);
    }
    // =========================
    // 🔍 SPHERE DETECTION
    // =========================
    private void DetectTree()
    {
        RaycastHit hit;

        Transform tr = transform;

        Vector3 origin = tr.position + Vector3.up;
        Vector3 direction = tr.forward;

        if (Physics.SphereCast(origin, detectionRadius, direction,
            out hit, detectionDistance, treeLayer))
        {
            PushableTree tree = hit.collider.GetComponent<PushableTree>();

            if (tree != null && !tree.IsFallen())
            {
                // 🔥 If new tree detected → reset state
                if (currentTree != tree)
                {
                    progress = 0f;
                    completed = false;
                }

                currentTree = tree;
                canPush = true;
                return;
            }
        }

        ResetPushState();
    }

    private void ResetPushState()
    {
        canPush = false;
        currentTree = null;
        progress = 0f;
        completed = false;   // ✅ VERY IMPORTANT

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(false);

        ElephantAnimation.Instance.PushAnim(false);
        StopVibration();
    }

    // =========================
    // 🔘 MASH INPUT
    // =========================
    public void OnMash()
    {
        if (!canPush || completed) return;

        if (!IsHoldingForward()) return; // ✅ must hold forward

        if (progressSlider != null)
            SetSliderVisible(true);

        elephantAnim.PushAnim(true);
        lastMashTime = Time.time;

        progress += pressAmount;
        progress = Mathf.Clamp(progress, 0, maxProgress);

       /* progress = Mathf.Min(progress + pressAmount, maxProgress);
        progress = Mathf.Max(progress - decayRate * Time.deltaTime, 0f);*/

        // 🎥 CAMERA SHAKE
        if (impulseSource != null)
            impulseSource.GenerateImpulse(shakeForce);

        if (progress >= maxProgress)
        {
            completed = true;
            currentTree.FallDown(GetPushDirection());
            OnCompleted();
        }
    }

    public void treeFallShake()
    {
        shakeForce = 0.7f;
        if (impulseSource != null)
            impulseSource.GenerateImpulse(shakeForce);
    }
    // =========================
    // 🧭 CHECK FORWARD INPUT
    // =========================
    private bool IsHoldingForward()
    {
        Vector2 move = Input.Instance.GetMovementVector();
        if (move == Vector2.zero) return false;

        Vector3 moveDir = new Vector3(move.x, 0, move.y).normalized;
        Vector3 forward = transform.forward;
        forward.y = 0f;

        float dot = Vector3.Dot(forward.normalized, moveDir);

        return dot > forwardDotThreshold;
    }

    // =========================
    private Vector3 GetPushDirection()
    {
        Vector3 dir = currentTree.transform.position - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }

    private float lastSliderValue = -1f;

    private void UpdateUI()
    {
        if (progressSlider == null) return;

        float value = progress / maxProgress;

        if (Mathf.Abs(value - lastSliderValue) < 0.001f)
            return;

        lastSliderValue = value;
        progressSlider.value = value;
    }

    private float lastVibration = -1f;
    private void UpdateVibration()
    {
        if (gamepad == null) return;

        float normalized = progress / maxProgress;
        float strength = Mathf.Lerp(minVibration, maxVibration, normalized);

        if (Mathf.Abs(strength - lastVibration) < 0.01f)
            return;

        lastVibration = strength;
        gamepad.SetMotorSpeeds(strength, strength);
    }

    private void StopVibration()
    {
        if (gamepad == null) return;
        gamepad.SetMotorSpeeds(0f, 0f);
    }

    private void OnCompleted()
    {
        StopVibration();
        NotificationUI.Instance.ShowMessage("Kumki alerted!");
        if (progressSlider != null)
            SetSliderVisible(false);

        progress = 0f;
    }



    // =========================
    // 🎯 DEBUG GIZMOS
    // =========================
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.aliceBlue;

        Vector3 origin = transform.position + Vector3.up * 2f;
        Vector3 direction = transform.forward * detectionDistance;

        Gizmos.DrawWireSphere(origin, detectionRadius);
        Gizmos.DrawLine(origin, origin + direction);
        Gizmos.DrawWireSphere(origin + direction, detectionRadius);
    }
}