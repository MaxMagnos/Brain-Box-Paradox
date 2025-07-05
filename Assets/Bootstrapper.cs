using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A data structure to define a single logical "Level," which can consist of multiple scenes.
/// The first scene in the list is considered the "primary" scene for lighting and activation.
/// </summary>
[System.Serializable]
public class Level
{
    public string levelName; // A descriptive name for the Inspector
    public List<string> sceneNames; // The actual scene files to load for this level
}

/// <summary>
/// Manages loading and unloading of game levels, including levels composed of multiple scenes.
/// This object persists across all scene loads to manage the game flow.
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }

    [Header("Scene Configuration")]
    [Tooltip("Define all game levels here. Each level can be made of one or more scenes.")]
    [SerializeField] private List<Level> gameLevels;

    private int currentLevelIndex = -1;

    private GameObject playerObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        LoadNextLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
    }

    /// <summary>
    /// Public method to initiate the transition to the next level.
    /// </summary>
    public void LoadNextLevel()
    {
        StartCoroutine(TransitionToNextScene());
    }

    private IEnumerator TransitionToNextScene()
    {
        // 1. UNLOAD the previous level's scenes.
        if (currentLevelIndex >= 0)
        {
            Level oldLevel = gameLevels[currentLevelIndex];
            Debug.Log($"Unloading level: {oldLevel.levelName}");
            foreach (string sceneName in oldLevel.sceneNames)
            {
                // Check if the scene is actually loaded before trying to unload it
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    yield return SceneManager.UnloadSceneAsync(sceneName);
                }
            }
        }

        // 2. INCREMENT the level index.
        currentLevelIndex++;

        // 3. CHECK if all levels are completed.
        if (currentLevelIndex >= gameLevels.Count)
        {
            Debug.Log("All levels completed!");
            yield break;
        }

        // 4. LOAD all scenes for the new level additively.
        Level newLevel = gameLevels[currentLevelIndex];
        Debug.Log($"Loading level: {newLevel.levelName}");
        foreach (string sceneName in newLevel.sceneNames)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        // 5. SET the primary scene of the new level as active.
        // We'll define the primary scene as the *first one* in the list.
        string primarySceneName = newLevel.sceneNames[0];
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(primarySceneName));
        Debug.Log($"Active scene set to: {primarySceneName}");
        
        //Finding Anchors in Scene and assigning them to the RoomSwitcher component
        var anchorA_Position = GameObject.FindGameObjectWithTag("Anchor_A").transform.position;
        var anchorB_Position = GameObject.FindGameObjectWithTag("Anchor_B")?.transform.position;
        if (anchorA_Position != null)
        {
            playerObject.GetComponent<RoomSwitcher>().SetAnchors(anchorA_Position, anchorB_Position);
        }
        
        
    }
}