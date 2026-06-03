using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera playerCam;

    private void Start()
    {
        playerCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (playerCam == null) return;

        transform.forward = playerCam.transform.forward;
    }
}