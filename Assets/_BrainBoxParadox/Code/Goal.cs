using System;
using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public event Action OnGoalAchieved;
    [SerializeField] private int requiredID;
    private SnapPoint snapPoint;
    
    [SerializeField] private Animator animator;
    
    private void Awake()
    {
        snapPoint = GetComponentInParent<SnapPoint>();
        animator = GetComponent<Animator>();
    }
    
    private void OnEnable()
    {
        snapPoint.OnObjectPlaced += CheckIfGoalAchieved;
    }

    private void OnDisable()
    {
        snapPoint.OnObjectPlaced -= CheckIfGoalAchieved;
    }

    private void CheckIfGoalAchieved(int placedShapeID, GameObject gameObject)
    {
        if (placedShapeID == requiredID)
        {
            gameObject.SetActive(false);
            StartCoroutine(GoalAchieved());
        }
        else
        {
            Debug.Log("GOAL NOT ACHIEVED");
        }
    }

    private IEnumerator GoalAchieved()
    {
        
        //Stupid ass if-statement for each of the goal types
        if (requiredID == 2)
        {
            animator.SetTrigger("PlayScrew");
        }
        else if (requiredID == 3)
        {
            animator.SetTrigger("PlaySwallow");
        }
        else if (requiredID == 4)
        {
            animator.SetTrigger("PlayStab");
        }
        
        AudioManager.Instance.PlaySound("PuzzleSolved", 1f);
        //OnGoalAchieved?.Invoke();
        yield return null;
    }

    public void CallGoalAchieved()
    {
        Debug.Log("CallGoalAchieved");
        OnGoalAchieved?.Invoke();
    }
}
