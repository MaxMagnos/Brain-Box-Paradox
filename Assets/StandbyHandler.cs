using System;
using UnityEngine;

public class StandbyHandler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }
}
