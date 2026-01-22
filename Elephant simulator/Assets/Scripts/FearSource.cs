using UnityEngine;

public class FearSource : MonoBehaviour
{
    public float maxIntensity = 3f;
    public float maxDistance = 20f;
    [SerializeField] private float thresholdIntensity = 1.5f;

    private bool runningBlocked;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        float distance = Vector3.Distance(transform.position, other.transform.position);
        float intensity = Mathf.Lerp(maxIntensity, 0f, distance / maxDistance);

        FearMeter.Instance.SetFearSource(true, intensity);

        if (intensity > thresholdIntensity && !runningBlocked)
        {
            runningBlocked = true;
            Input.Instance.StopRunning();
        }
        else if (intensity <= thresholdIntensity && runningBlocked)
        {
            runningBlocked = false;
            if (Input.Instance.isSprintPressed())
                Input.Instance.StartRunning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        FearMeter.Instance.SetFearSource(false, 0f);
        runningBlocked = false;
    }

    // 💥 Proper shutdown
    public void DisableFearSource()
    {
        FearMeter.Instance.SetFearSource(false, 0f);
        runningBlocked = false;
        GetComponent<SphereCollider>().enabled = false;
        enabled = false;
    }
}
