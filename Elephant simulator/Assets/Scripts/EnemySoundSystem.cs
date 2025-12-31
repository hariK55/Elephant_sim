using UnityEngine;

public class EnemySoundSystem : MonoBehaviour
{
    public static void EmitSound(Vector3 position, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius);
        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.HearSound(position);
            }
        }
    }
}
