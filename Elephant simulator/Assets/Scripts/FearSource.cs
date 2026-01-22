using UnityEngine;

public class FearSource : MonoBehaviour
{
    public float maxIntensity = 3f;
    public float minDistance = 1f;
    public float maxDistance = 20f;
    float distance;
    float intensity;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            distance = Vector3.Distance(transform.position, other.transform.position);
             intensity = Mathf.Lerp(maxIntensity, 0.2f, distance / maxDistance);
          

            FearMeter.Instance.SetFearSource(true, intensity);
            if (intensity > 1f)
            {
                Input.Instance.StopRunning();
            }
            else if(Input.Instance.isSprintPressed()) 
                Input.Instance.StartRunning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FearMeter.Instance.SetFearSource(false, 1f);
        }
    }
}
