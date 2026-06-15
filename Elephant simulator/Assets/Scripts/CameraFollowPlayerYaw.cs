using UnityEngine;

public class CameraFollowPlayerYaw : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 3f;

    void LateUpdate()
    {
        Vector3 euler = transform.eulerAngles;

        float targetY = player.eulerAngles.y;

        euler.y = Mathf.LerpAngle(
            euler.y,
            targetY,
            followSpeed * Time.deltaTime);

        transform.eulerAngles = euler;
    }
}