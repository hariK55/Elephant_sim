using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ElephantAnimation : MonoBehaviour
{
    public static ElephantAnimation Instance { get; private set; }
    private Animator animator;

    float start = 0.6f;
    float end = 0.6f;
    float currentT = 0.2f;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AnimationController();
        //LoopAnim(0.6f, 0.6f, 0.2f, 0.2f, "startSlide", Input.Instance.isSlidingDownhill && Input.Instance.IsRunning());
    }
    private void AnimationController()
    {
        animator.SetBool("isWalking", Input.Instance.IsWalking());

        animator.SetBool("isRunning", Input.Instance.IsRunning() && Input.Instance.IsWalking());


       // animator.SetBool("isSliding", Input.Instance.isSlidingDownhill);
       //sliding animation
        if (Input.Instance.isSlidingDownhill && Input.Instance.IsRunning())
        {
            // step through the normalized time
            currentT += Time.deltaTime * 1f; // speed 1x

            // wrap back to start if passed end
            if (currentT > end)
                currentT = start;

            animator.Play("startSlide", 0, currentT);
        }
        else
        {
            currentT = 0.2f;
        }
       
   
        
    }
    public void pick()
    {
        animator.Play("pick");
    }

    public void dropAnim()
    {
        animator.SetTrigger("drop");
    }
    public void eatAnim()
    {
        animator.SetTrigger("eat");
    }

}
