using System;
using UnityEditor.Rendering;
using UnityEngine;

public class Input : MonoBehaviour
{
    [SerializeField]
    public bool isInteract;
    [SerializeField] private float walkSound = 10f;
    [SerializeField] private float runSound = 20f;


    [Header("Slope Detection")]
    [SerializeField] private float steepSlopeAngle = 30f;

    public bool isSteepSlope;         // just detects slope > angle
    public bool isSlidingDownhill;    // detects downhill movement
    public bool isGrounded;
    private Vector3 slopeNormal;
    private float currentSlopeAngle;

    // [SerializeField] private Vector3 euler ;
    public static Input Instance { get; private set; }

    public event EventHandler OnInteractPressed;

    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float walkSpeed = 3.2f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float slopeStickForce = 3f;
   // [SerializeField] private float slopeAlignSpeed = 2f;
    [SerializeField] private Transform cameraTransform;
    private float moveSpeed;
    private float speed;
    private Animator animator;
    private Rigidbody rb;

    private bool isWalking;
    private bool isRunning;

    InputSystem inputActions;

   /* float maxtime = 2.5f;
    float currentT;*/

    private void Awake()
    {
        Instance = this;
        inputActions = new InputSystem();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        moveSpeed = walkSpeed;
        // Prevent the player from falling sideways
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        speed = moveSpeed;
    }

    void Start()
    {
        inputActions.Player.Enable();
        inputActions.Player.Sprint.performed += Sprint_performed;
        inputActions.Player.Sprint.canceled += Sprint_canceled;
        inputActions.Player.Interact.performed += Interact_performed;
        //inputActions.Player.Interact.canceled += Interact_canceled;
        inputActions.Player.Eat.started += Eat_started;
        inputActions.Player.Eat.canceled += Eat_canceled;
        inputActions.Player.Attack.performed += Attack_performed;
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        RapidPressMechanic.Instance.OnMash();
       
    }
    
    private void Eat_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        EatUI.Instance.OnHoldCanceled();
    }

    private void Eat_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        EatUI.Instance.OnHold();
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       // isInteract = true;
        OnInteractPressed?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        speed = moveSpeed;
        isRunning = false;
    }

    private void Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        speed = runSpeed;
      
        isRunning = true;
    }

    private void FixedUpdate()
    {
        //euler = slopeRotation.eulerAngles;
        Movement();
        StickToSlope();
        AlignRotationToSlope();
        CheckSlopeStatus();

        if (isSteepSlope)
        {
            moveSpeed = 4f;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        
         if(isRunning)
            EnemySoundSystem.EmitSound(transform.position, runSound);
        else if (isWalking)
            EnemySoundSystem.EmitSound(transform.position, walkSound);

    }

    // ---------------------------------------------------
    // CAMERA-RELATIVE MOVEMENT
    // ---------------------------------------------------
    private void Movement()
    {
        if (!isGrounded) return;

        if (!ElephantAnimation.Instance.getFalling()) return;

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
        Vector3 targetVelocity = moveDir * speed;
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
    [SerializeField] private float normalSmoothSpeed = 5.25f;
    [SerializeField] private float slopeRotationSmooth = 10.25f;

    private void AlignRotationToSlope()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position + (Vector3.up * 0.3f) + (transform.forward * 1.2f), Vector3.down, Color.blue);
        if (Physics.Raycast(transform.position + (Vector3.up * 0.3f), Vector3.down, out hit, 2f))
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

    public bool IsRunning() => isRunning;

    private void CheckSlopeStatus()
    {
        RaycastHit hit;

        // check if grounded
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.5f,
            Vector3.down,
            out hit,
            7f
        );
        //fall animation
        if (!isGrounded)
        {
            ElephantAnimation.Instance.Fall(true);

        }
        else
        {
            ElephantAnimation.Instance.Fall(false);
        }
      
        if (!isGrounded)
        {
            isSteepSlope = false;
            isSlidingDownhill = false;
            return;
        }

        slopeNormal = hit.normal;
        currentSlopeAngle = Vector3.Angle(slopeNormal, Vector3.up);

        // ---------------------------------------------------------
        // 1. detect steep slope (>45 deg)
        // ---------------------------------------------------------
        isSteepSlope = currentSlopeAngle > steepSlopeAngle;

        if (!isSteepSlope)
        {
            isSlidingDownhill = false;
            return;
        }

        // ---------------------------------------------------------
        // 2. detect downhill direction
        // ---------------------------------------------------------
        // vector that points DOWN the slope
        Vector3 slopeDownDir = Vector3.ProjectOnPlane(Vector3.down, slopeNormal).normalized;

        // player movement vector
        Vector3 horizontalVel = rb.linearVelocity;
        horizontalVel.y = 0;

        float downhillDot = Vector3.Dot(horizontalVel.normalized, slopeDownDir);

        // If dot > 0.5 → moving downhill
        isSlidingDownhill = downhillDot > 0.5f && horizontalVel.magnitude > 0.2f;
    }

}
