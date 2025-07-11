using System;
using UnityEngine;
using TMPro;

public class VolumeBar : MonoBehaviour
{
    private RoomSyncHandler roomSyncHandler;
    
    [SerializeField] private TextMeshProUGUI barText;
    [SerializeField] private int barLength;
    [SerializeField] private GameObject pausedPanel;
    
    private float calibrationTime;
    private string barString;


    private void UpdateBar()
    {
        if (roomSyncHandler.GetSyncRate() < 100 && !pausedPanel.activeInHierarchy)
        {
            pausedPanel.SetActive(true);
        }
        else if (roomSyncHandler.GetSyncRate() > 100 && pausedPanel.activeInHierarchy)
        {
            pausedPanel.SetActive(false);
        }
        
        calibrationTime = roomSyncHandler.GetCalibrationTime();
        var percentage = calibrationTime / 60f;

        barString = "";
        for (int i = 0; i < barLength; i++)
        {
            if (percentage * barLength > i)
            {
                barString += "|";
            }
            else
            {
                barString += "-";
            }
        }
        
        barText.text = barString;
    }

    private void Start()
    {
        roomSyncHandler = GameObject.FindGameObjectWithTag("RoomSyncHandler").GetComponent<RoomSyncHandler>();
        pausedPanel.SetActive(false);
    }

    private void Update()
    {
        UpdateBar();
    }
}
