using System;
using UnityEngine;

public class KeepAcrossScenes : MonoBehaviour
{
    [SerializeField] private bool isActive;
    
    private void Awake()
    {
        if (isActive)
        {
            DontDestroyOnLoad(this);
        }
    }
}
