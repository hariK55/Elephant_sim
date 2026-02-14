using UnityEngine;

public class VehicleAI_Complete : MonoBehaviour
{
    public enum DriverType
    {
        Careful,
        Honker,
        Aggressive
    }

    [Header("Driver Type")]
    public DriverType driverType;

    [Header("Lane Following")]
    public Transform[] waypoints;
    public float laneSpeed = 12f;
    public float rotationSpeed = 5f;

    [Header("Detection")]
    public Transform elephant;
    public LayerMask elephantLayer;
    public float detectDistance = 18f;
    public float stopDistance = 12f;
    public float safeDistance = 25f;

    [Header("Aggressive Settings")]
    public float offRoadSpeed = 18f;
    public float fleeSpeed = 22f;
    public float offRoadDuration = 3f;

    [Header("Audio")]
    public AudioSource hornAudio;
    [SerializeField] private AudioSource engineSound;

    private int currentWaypoint = 0;
    private float currentSpeed;
    private bool isStopped = false;
    private bool isOffRoad = false;
    private float offRoadTimer = 0f;

   [SerializeField] private Rigidbody rb;
    private bool isDestroyed = false;

    public float flipThreshold = 0.3f;  // how tilted before considered fallen

    void Start()
    {
        if(rb==null)
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(isDestroyed) return;
        if (elephant == null || waypoints.Length == 0)
            return;

        CheckIfFallen();

        CheckForElephant();

        if (driverType == DriverType.Aggressive && isOffRoad)
        {
            HandleOffRoad();
        }
        else
        {
            FollowLane();
        }

        ResumeCheck();
    }
     [SerializeField]float dot;
    void CheckIfFallen()
    {
        if (isDestroyed) return;

         dot = Vector3.Dot(rb.transform.up, Vector3.up);

        Debug.Log(dot);
        if (dot <= flipThreshold)
        {
            PermanentlyStopVehicle();
        }
    }

    void PermanentlyStopVehicle()
    {
        isDestroyed = true;

        // Stop movement logic
        isStopped = true;
        isOffRoad = false;

        // Disable AI movement
        this.enabled = false;
        hornAudio.Stop();
        engineSound.Stop();
        // Optional: Add more drag so it doesn't slide forever
        /* rb.drag = 3f;
         rb.angularDrag = 5f;*/
    }

    // ------------------ LANE FOLLOW ------------------

    void FollowLane()
    {
        float targetSpeed = isStopped ? 0f : laneSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 3f);

        Transform target = waypoints[currentWaypoint];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * currentSpeed * Time.deltaTime;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 3f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    // ------------------ ELEPHANT DETECTION ------------------

    void CheckForElephant()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up,
                            transform.forward,
                            out hit,
                            detectDistance,
                            elephantLayer))
        {
            float dist = Vector3.Distance(transform.position, elephant.position);

            switch (driverType)
            {
                case DriverType.Careful:
                    if (dist < stopDistance)
                    {
                        isStopped = true;
                        engineSound.Stop();
                    }
                        
                    break;

                case DriverType.Honker:
                    if (dist < stopDistance)
                    {
                        isStopped = true;
                        PlayHorn();
                    }
                    break;

                case DriverType.Aggressive:
                   /* if (dist < stopDistance)
                    {
                        isOffRoad = true;
                        offRoadTimer = offRoadDuration;
                    }*/
                    break;
            }
        }
    }

    // ------------------ OFFROAD BEHAVIOR ------------------

    void HandleOffRoad()
    {
        offRoadTimer -= Time.deltaTime;

        Vector3 fleeDirection = (transform.position - elephant.position).normalized;

        // If elephant chasing → run faster
        float distance = Vector3.Distance(transform.position, elephant.position);
        float speed = distance < 10f ? fleeSpeed : offRoadSpeed;

        transform.position += fleeDirection * speed * Time.deltaTime;

        Quaternion rot = Quaternion.LookRotation(fleeDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

        if (offRoadTimer <= 0 && distance > safeDistance)
        {
            isOffRoad = false;
        }
    }

    // ------------------ RESUME LOGIC ------------------

    void ResumeCheck()
    {
        float dist = Vector3.Distance(transform.position, elephant.position);

        if (driverType != DriverType.Aggressive && dist > safeDistance)
        {
            isStopped = false;
            engineSound.Play();
            if(hornAudio != null && hornAudio.isPlaying)
            {
                hornAudio.Stop();
            }
        }
    }

    // ------------------ HORN ------------------

    void PlayHorn()
    {
        if (hornAudio != null && !hornAudio.isPlaying)
        {
            hornAudio.Play();
        }
    }

    // ------------------ GIZMOS ------------------

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up,
                       transform.forward * detectDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
