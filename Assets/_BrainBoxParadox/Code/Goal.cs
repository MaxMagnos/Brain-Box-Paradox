using System;
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
            OnGoalAchieved?.Invoke();
            Debug.Log("GOAL ACHIEVED");
        }
        else
        {
            Debug.Log("GOAL NOT ACHIEVED");
        }
    }
}
