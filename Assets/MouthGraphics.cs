using System;
using UnityEngine;

public class MouthGraphics : MonoBehaviour
{
    private Animator animator;
    
    private Goal goal;

    private void Start()
    {
        animator = GetComponent<Animator>();
        goal = GetComponentInParent<Goal>();

        goal.OnGoalAchieved += PlaySwallowAnimation;
    }


    private void PlaySwallowAnimation()
    {
        animator.SetTrigger("PlaySwallow");
    }

    public void ToungeOutAnimationStarted()
    {
        AudioManager.Instance.PlaySound("MouthOpen", transform.position, 1f);
    }
}
 