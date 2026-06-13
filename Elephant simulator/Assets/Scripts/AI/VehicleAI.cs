using UnityEngine;
using System.Collections;

public class VehicleAI_Complete : MonoBehaviour
{
    public enum DriverType { Careful, Honker, Aggressive }

    public FearSource fearSource;

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

    [Header("Audio")]
    public AudioSource hornAudio;
    [SerializeField] private AudioSource engineSound;

    [Header("Physics")]
    [SerializeField] private Rigidbody rb;

    [Header("State")]
    private int currentWaypoint;
    private float currentSpeed;
    private bool isStopped;
    private bool isDestroyed;

    private Transform cachedTransform;

    void Start()
    {
        cachedTransform = transform;

        if (!rb) rb = GetComponent<Rigidbody>();
        if (!fearSource) fearSource = GetComponent<FearSource>();

        if (engineSound) engineSound.Play();

        StartCoroutine(AI_Loop());
    }

    IEnumerator AI_Loop()
    {
        // stagger start to avoid all vehicles syncing
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));

        WaitForSeconds tick = new WaitForSeconds(0.2f);

        while (!isDestroyed)
        {
            if (elephant != null && waypoints.Length > 0)
            {
                CheckIfFallen();
                CheckForElephant();
                ResumeCheck();
            }

            yield return tick;
        }
    }

    void CheckIfFallen()
    {
        float dot = Vector3.Dot(rb.transform.up, Vector3.up);

        if (dot <= 0.3f)
            PermanentlyStopVehicle();
    }

    void CheckForElephant()
    {
        Vector3 origin = cachedTransform.position + Vector3.up;

        if (Physics.Raycast(origin, cachedTransform.forward, out RaycastHit hit, detectDistance, elephantLayer))
        {
            float dist = Vector3.Distance(cachedTransform.position, elephant.position);

            if (driverType == DriverType.Careful)
            {
                if (dist < stopDistance)
                {
                    isStopped = true;
                    if (fearSource) fearSource.DisableFearSource();
                    if (engineSound) engineSound.Stop();
                }
            }
            else if (driverType == DriverType.Honker)
            {
                if (dist < stopDistance)
                {
                    isStopped = true;
                    PlayHorn();
                }
            }
        }
    }

    void ResumeCheck()
    {
        float dist = Vector3.Distance(cachedTransform.position, elephant.position);

        if (driverType != DriverType.Aggressive && dist > safeDistance)
        {
            if (fearSource) fearSource.EnableFearSource();
            isStopped = false;

            if (engineSound && !engineSound.isPlaying)
                engineSound.Play();

            if (hornAudio && hornAudio.isPlaying)
                hornAudio.Stop();
        }
    }

    void FixedUpdate()
    {
        if (isDestroyed || waypoints.Length == 0) return;
        FollowLane();
    }

    void FollowLane()
    {
        Transform target = waypoints[currentWaypoint];

        Vector3 dir = (target.position - cachedTransform.position).normalized;

        float targetSpeed = isStopped ? 0f : laneSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.1f);

        cachedTransform.position += dir * currentSpeed * Time.fixedDeltaTime;

        Quaternion rot = Quaternion.LookRotation(dir);
        cachedTransform.rotation =
            Quaternion.Slerp(cachedTransform.rotation, rot, rotationSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(cachedTransform.position, target.position) < 3f)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void PermanentlyStopVehicle()
    {
        isDestroyed = true;
        isStopped = true;

        if (hornAudio) hornAudio.Stop();
        if (engineSound) engineSound.Stop();

        enabled = false;
    }

    void PlayHorn()
    {
        if (hornAudio && !hornAudio.isPlaying)
            hornAudio.Play();
    }
}