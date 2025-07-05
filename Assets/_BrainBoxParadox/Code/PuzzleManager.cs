using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    public event Action OnPuzzleCompleted;

    private Goal goal;

    public void Initialize(PuzzleData data, Transform[] spawnPoints)
    {
        // Convert the array to a List for easy removal of used points.
        var remainingSpawnPoints = new System.Collections.Generic.List<Transform>(spawnPoints);

        foreach (var obj in data.puzzleObjects)
        {
            // Check if there are any spawn points left.
            if (remainingSpawnPoints.Count == 0)
            {
                Debug.LogWarning("Not enough spawn points for all puzzle objects. Some were not spawned.");
                break;
            }

            // Pick a random index from the current list of available points.
            int pointIndex = Random.Range(0, remainingSpawnPoints.Count);
            Transform spawnTransform = remainingSpawnPoints[pointIndex];

            // Instantiate the object at the chosen position.
            Instantiate(obj, spawnTransform.position, Quaternion.identity);
        
            // Remove the used spawn point from the list so it can't be chosen again.
            remainingSpawnPoints.RemoveAt(pointIndex);
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
