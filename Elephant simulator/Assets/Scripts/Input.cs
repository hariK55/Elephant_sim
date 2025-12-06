using UnityEngine;

public class Input : MonoBehaviour
{
    public static Input Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float slopeStickForce = 8f;
    [SerializeField] private float slopeAlignSpeed = 10f;
    [SerializeField] private Transform cameraTransform;

    private Animator animator;
    private Rigidbody rb;

    private bool isWalking;

    InputSystem inputActions;

    private void Awake()
    {
        Instance = this;
        inputActions = new InputSystem();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Prevent the player from falling sideways
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        inputActions.Player.Enable();
    }

    private void FixedUpdate()
    {
        Movement();
        StickToSlope();
        AlignRotationToSlope();
    }

    // ---------------------------------------------------
    // CAMERA-RELATIVE MOVEMENT
    // ---------------------------------------------------
    private void Movement()
    {
        Vector2 moveInput = GetMovementVector();

        // Convert to world space using camera
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;

        // Apply velocity
        Vector3 targetVelocity = moveDir * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y; // keep Unity gravity
        rb.linearVelocity = targetVelocity;

        // Character rotation
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );

            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    // ---------------------------------------------------
    // SLOPE STICKING — PREVENT SLIDING
    // ---------------------------------------------------
    private void StickToSlope()
    {
        if (IsOnSlope())
        {
            rb.AddForce(Vector3.down * slopeStickForce, ForceMode.Force);
        }
    }

    private bool IsOnSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 1.3f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > 0 && angle < 45f;
        }
        return false;
    }

    // ---------------------------------------------------
    // ALIGN CHARACTER ROTATION TO SLOPE NORMAL
    // ---------------------------------------------------
    // Smoother slope alignment variables
    private Vector3 smoothedNormal = Vector3.up;
    [SerializeField] private float normalSmoothSpeed = 2f;
    [SerializeField] private float slopeRotationSmooth = 3f;

    private void AlignRotationToSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.3f, Vector3.down, out hit, 1.5f))
        {
            // STEP 1: Smooth the ground normal itself
            smoothedNormal = Vector3.Slerp(
                smoothedNormal,
                hit.normal,
                normalSmoothSpeed * Time.deltaTime
            );

            // STEP 2: Find slope rotation using smoothed normal
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, smoothedNormal) * transform.rotation;

            // STEP 3: Smoothly apply that rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                slopeRotation,
                slopeRotationSmooth * Time.deltaTime
            );
        }
        else
        {
            // No ground detected? Smoothly return to upright
            smoothedNormal = Vector3.Slerp(
                smoothedNormal,
                Vector3.up,
                normalSmoothSpeed * Time.deltaTime
            );
        }
    }


    // ---------------------------------------------------
    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        return inputVector.normalized;
    }

    public bool IsWalking() => isWalking;
}
