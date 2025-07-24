using System;
using UnityEngine;

public class Room1Music : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayMusic("Music1", 0.5f, true);
    }
}
