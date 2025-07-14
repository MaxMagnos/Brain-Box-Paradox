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
        
        

        Transform anchorA = GameObject.FindGameObjectWithTag("Anchor_A").transform;
        // Find Anchor B's transform, but safely handle the case where it might not exist.
        GameObject anchorB_GO = GameObject.FindGameObjectWithTag("Anchor_B");
        Transform anchorB = null;
        if (anchorB_GO != null)
        {
            anchorB = anchorB_GO.transform;
        }

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
            GameObject newInstance = Instantiate(obj, spawnTransform.position, Quaternion.identity, this.transform);
            
            //Add newInstance to puzzleComponents
            puzzleComponents.Add(newInstance);

            //Rotation Related
            Vector3 nearestAnchorPos = anchorA.position;  //Default case is looking at AnchorA
            if (anchorB != null)
            {
                float distToA = Vector3.Distance(newInstance.transform.position, anchorA.position);
                float distToB = Vector3.Distance(newInstance.transform.position, anchorB.position);

                if (distToB < distToA)
                {
                    nearestAnchorPos = anchorB.position;
                }
            }
            
            Vector3 directionToAnchor = newInstance.transform.position - nearestAnchorPos;

            if (directionToAnchor != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToAnchor);
                newInstance.transform.rotation = lookRotation;
            }
            
            
            // Remove the used spawn point from the list so it can't be chosen again.
            remainingSpawnPoints.RemoveAt(pointIndex);
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
