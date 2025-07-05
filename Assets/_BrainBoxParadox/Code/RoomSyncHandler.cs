using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSyncHandler : MonoBehaviour
{
    [SerializeField] private PuzzleData[] puzzles;
    [SerializeField] private GameObject puzzleManagerPrefab;
    
    [SerializeField] public Transform[] spawnPoints;
    
    private PuzzleManager puzzleManager;

    private void Start()
    {
        GatherSpawnPoints();
        SpawnNextPuzzle();
    }

    private void SpawnNextPuzzle()
    {
        if (puzzles.Length == 0)
        {
            Debug.LogError("Puzzle Data is empty!");
            return;
        }
        
        PuzzleData nextPuzzle = puzzles[Random.Range(0, puzzles.Length)];
        
        GameObject puzzleManagerObj = Instantiate(puzzleManagerPrefab);
        puzzleManager = puzzleManagerObj.GetComponent<PuzzleManager>();
        
        //Subscribe to puzzle completed here
        //
        
        puzzleManager.Initialize(nextPuzzle, spawnPoints);
    }

    /// <summary>
    /// Finds all immediate children with the "SpawnPoint" tag 
    /// and populates the spawnPoints array with their transforms.
    /// </summary>
    private void GatherSpawnPoints()
    {
        // Use a List because we don't know the size in advance.
        List<Transform> foundPoints = new List<Transform>();

        // Iterate through each immediate child of this object.
        foreach (Transform child in transform)
        {
            // Check if the child has the correct tag.
            // CompareTag is more efficient than checking child.tag == "SpawnPoint".
            if (child.CompareTag("SpawnPoint"))
            {
                foundPoints.Add(child);
            }
        }

        // Convert the List to the final array.
        spawnPoints = foundPoints.ToArray();

        Debug.Log($"Gathered {spawnPoints.Length} spawn points.");
    }
}
