using UnityEngine;

public class HandGraphics : MonoBehaviour
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
        animator.SetTrigger("HandStab");
    }
    
}
