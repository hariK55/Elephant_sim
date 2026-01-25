using UnityEngine;

public class banana : MonoBehaviour
{
   

    public void set()
    {
        gameObject.transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
