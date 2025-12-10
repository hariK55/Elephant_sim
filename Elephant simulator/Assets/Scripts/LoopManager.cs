using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    private Animator animator;

    private string animName;
    private float startT;
    private float endT;
    private float FirstStart;
    private float currentT;
    private bool looping = false;

    void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!looping) return;

        // Increase normalized time
        currentT += Time.deltaTime;

        if (currentT > endT)
            currentT = startT; // loop back

        animator.Play(animName, 0, currentT);
    }

    /// <summary>
    /// Start looping any part of any animation.
    /// </summary>
    public void Loop(string animationName, float startNormalized, float endNormalized, float FirstStart = 0f)
    {
        animName = animationName;
        startT = startNormalized;
        endT = endNormalized;

        currentT =FirstStart;
        looping = true;
    }

    /// <summary>
    /// Stop looping and return Animator control.
    /// </summary>
    public void StopLoop()
    {
        looping = false;
        currentT = FirstStart;

    }
}

