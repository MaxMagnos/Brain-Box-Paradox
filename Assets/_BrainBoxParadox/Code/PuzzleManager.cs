using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    public event Action OnPuzzleCompleted;

    private List<GameObject> puzzleComponents = new List<GameObject>();

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
            remainingSpawnPoints.RemoveAt(pointIndex);

            // Instantiate the object at the chosen position, as a child of the PuzzleManager and adding it to List.
            puzzleComponents.Add(Instantiate(obj, spawnTransform.position, Quaternion.identity, this.transform));

        }

        goal = GetComponentInChildren<Goal>();
        if (goal != null)
        {
            goal.OnGoalAchieved += HandleGoalAchieved;
        }
        else
        {
            Debug.LogWarning("No goal found.");
        }
    }

    public void HandleGoalAchieved()
    {
        StartCoroutine(DestroyWithAnimation());
    }

    private void OnDestroy()
    {
        if (goal)
        {
            goal.OnGoalAchieved -= HandleGoalAchieved;
        }
    }

    private IEnumerator DestroyWithAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var obj in puzzleComponents)
        {
            if (obj != null)
            {
                var popEffect = obj.GetComponent<PopEffect>();
                if (popEffect != null)
                {
                    popEffect.DestroyWithPop();
                }
            }
        }
        //this.GetComponent<PopEffect>().DestroyWithPop();  //Not used since PuzzleManager does not have a popEffect, as that would double it on all children.
        yield return new WaitForSeconds(0.5f);
        OnPuzzleCompleted?.Invoke();
        Destroy(this.gameObject);
    }
}
