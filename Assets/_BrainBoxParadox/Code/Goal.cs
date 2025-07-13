using System;
using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public event Action OnGoalAchieved;
    [SerializeField] private int requiredID;
    private SnapPoint snapPoint;
    
    private void Awake()
    {
        snapPoint = GetComponent<SnapPoint>();
    }
    
    private void OnEnable()
    {
        snapPoint.OnObjectPlaced += CheckIfGoalAchieved;
    }

    private void OnDisable()
    {
        snapPoint.OnObjectPlaced -= CheckIfGoalAchieved;
    }

    private void CheckIfGoalAchieved(int placedShapeID)
    {
        if (placedShapeID == requiredID)
        {
            StartCoroutine(GoalAchieved());
        }
        else
        {
            Debug.Log("GOAL NOT ACHIEVED");
        }
    }

    private IEnumerator GoalAchieved()
    {
        AudioManager.Instance.PlaySound("PuzzleSolved", 1f);
        
        OnGoalAchieved?.Invoke();
        yield return null;
    }
}
