using UnityEngine;
using System.Collections.Generic;

public class EnemySoundSystem : MonoBehaviour
{
    // -------- CONFIG --------
    public static float gizmoDisplayTime = 1.5f;

    // -------- SOUND DATA --------
    private class SoundEvent
    {
        public Vector3 position;
        public float radius;
        public float time;
    }

    private static List<SoundEvent> activeSounds = new List<SoundEvent>();

    // -------- EMIT SOUND --------
    public static void EmitSound(Vector3 position, float radius)
    {
        // store sound for gizmos
        activeSounds.Add(new SoundEvent
        {
            position = position,
            radius = radius,
            time = Time.time
        });

        Collider[] hits = Physics.OverlapSphere(position, radius);

        foreach (Collider hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.HearSound(position);
            }
        }
    }

    // -------- GIZMOS --------
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        float now = Time.time;

        for (int i = activeSounds.Count - 1; i >= 0; i--)
        {
            SoundEvent s = activeSounds[i];

            // remove expired sounds
            if (now - s.time > gizmoDisplayTime)
            {
                activeSounds.RemoveAt(i);
                continue;
            }

            // draw sound radius
            Gizmos.color = new Color(0f, 0.7f, 1f, 0.25f);
            Gizmos.DrawSphere(s.position, s.radius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(s.position, s.radius);
        }
    }
}
