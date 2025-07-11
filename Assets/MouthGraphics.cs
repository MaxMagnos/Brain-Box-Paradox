using System;
using UnityEngine;

public class MouthGraphics : MonoBehaviour
{
    private Animator animator;
    
    private Goal goal;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        goal = GetComponent<Goal>();

        goal.OnGoalAchieved += PlaySwallowAnimation;
    }


    private void PlaySwallowAnimation()
    {
        animator.SetTrigger("PlaySwallow");
    }
}
 