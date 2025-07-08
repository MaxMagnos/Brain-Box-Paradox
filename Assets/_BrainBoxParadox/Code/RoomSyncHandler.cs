using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSyncHandler : MonoBehaviour
{
    public event Action OnCalibrationCompleted;
    
    [SerializeField] private PuzzleData[] puzzles;
    [SerializeField] private GameObject puzzleManagerPrefab;
    
    [SerializeField] public Transform[] spawnPoints;
    
    [Header("Sync Rate Related")]
    [Tooltip("Current SyncRate. >100 makes calibration fill up.")]
    [SerializeField] private float syncRate;
    
    [Tooltip("Amount of SyncRate increase per solved puzzle.")]
    [SerializeField] private float syncRateIncrease;
    
    [Tooltip("Amount of SyncRate decrease per second.")]
    [SerializeField] private float syncRateDecay;
    
    [Header("Calibration Related")]
    [SerializeField] private float calibrationTime;
    [SerializeField] private bool calibrationComplete = false;
    
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
        puzzleManager.OnPuzzleCompleted += PuzzleComplete;
        
        puzzleManager.Initialize(nextPuzzle, spawnPoints);
    }
    private void PuzzleComplete()
    {
        puzzleManager.OnPuzzleCompleted -= PuzzleComplete;
        syncRate += syncRateIncrease;
        
        
        if(calibrationComplete) {return;}
        
        SpawnNextPuzzle();
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


    private void Update()
    {
        if(calibrationComplete) {return;}
        
        if (syncRate > 100)
        {
            calibrationTime += Time.deltaTime;
            if (calibrationTime >= 60f)
            {
                StartCoroutine(CalibrationComplete());
            }
        }
        
        syncRate = Mathf.Max(0f, syncRate - (syncRateDecay * Time.deltaTime));
    }

    private IEnumerator CalibrationComplete()
    {
        calibrationComplete = true;
        puzzleManager.OnPuzzleCompleted -= PuzzleComplete;
        puzzleManager.HandleGoalAchieved();
        
        //TODO: Add some sound here
        
        yield return new WaitForSeconds(3f);
        OnCalibrationCompleted?.Invoke();
        
        Debug.Log("Calibration for this Room is complete.");
    }
}
