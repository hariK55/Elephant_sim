using Unity.VisualScripting;
using UnityEngine;


public class Interactor : MonoBehaviour
{
    public float interactDistance = 10f;
    //public LayerMask interactLayer;
    [SerializeField] private float radius = 0.3f;
    [SerializeField] private float distance = 10f;

    void Update()
    {
        if (true)
        {
           // Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, distance))
            {
                Iinteractable interactable = hit.collider.GetComponent<Iinteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        // Visualise the ray
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.green);
    }
}
  
