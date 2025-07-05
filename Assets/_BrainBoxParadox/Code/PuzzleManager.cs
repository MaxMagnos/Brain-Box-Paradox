using System;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public event Action OnPuzzleCompleted;

    private Goal goal;

    public void Initialize(PuzzleData data)
    {
        foreach (var obj in data.puzzleObjects)
        {
            Instantiate(obj, obj.transform.position, Quaternion.identity);
        }

        goal = GetComponentInChildren<Goal>();
        if (goal != null)
        {
            goal.OnGoalAchieved += HandleGoalAchieved;
        }
    }

    private void HandleGoalAchieved()
    {
        OnPuzzleCompleted?.Invoke();
    }

    private void OnDestroy()
    {
        if (goal)
        {
            goal.OnGoalAchieved -= HandleGoalAchieved;
        }
    }
}
