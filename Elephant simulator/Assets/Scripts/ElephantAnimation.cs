using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class ElephantAnimation : MonoBehaviour
{
    public static ElephantAnimation Instance { get; private set; }
    public Animator animator;

    float start = 0.6f;
    float end = 0.6f;
    float currentT = 0.2f;

    bool canMove = true;

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
        if (Input.Instance.caught) return;
        AnimationController();
       
    }
    private void AnimationController()
    {
        animator.SetBool("isWalking", Input.Instance.IsWalking());

        animator.SetBool("isRunning", Input.Instance.IsRunning() && Input.Instance.IsWalking());


       
       //sliding animation
        if (Input.Instance.isSlidingDownhill && Input.Instance.IsRunning())
        {
            // step through the normalized time
            currentT += Time.deltaTime * 1f; // speed 1x

            // wrap back to start if passed end
            if (currentT > end)
                currentT = start;

            animator.Play("startSlide", 0, currentT);
            //SoundManager.Instance.PlaySfx(Sound.slide, 0.5f);
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
    public void eatAnim(bool iseating)
    {
        animator.SetBool("eat",iseating);
    }
    public void PushAnim(bool isPush)
    {
        animator.SetBool("isPushing", isPush);
    }
    public bool getPushing()
    {
        return animator.GetBool("isPushing");
    }
    public void IsCane(bool isCane)
    {
        animator.SetBool("isCane", isCane);
    }

    public void Fall(bool fall)
    {

        animator.SetBool("fall",fall);
        if(fall)
        {
            StartCoroutine(WaitForAnimation(7.5f));
        }
         
    }

    public bool getFalling()
    {
        return canMove;
    }
    IEnumerator WaitForAnimation(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }

    public void Attack()
    {
        animator.SetTrigger("attack");
    }

    public void HoldAttack(bool holding)
    {
        animator.SetTrigger("holding");
    }

    public void Caught()
    {
        animator.SetTrigger("caught");
    }

    public void Trumpet()
    {
        animator.SetTrigger("trumpet");
    }

    public void Sleep()
    {
       bool sleepToggle= animator.GetBool("sleep");
        animator.SetBool("sleep", !sleepToggle);
    }
}
