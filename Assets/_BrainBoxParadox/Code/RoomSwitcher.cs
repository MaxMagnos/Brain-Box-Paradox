using System;
using Unity.VisualScripting;
using UnityEngine;

enum RoomVariant
{
    A,
    B
}

public class RoomSwitcher : MonoBehaviour
{
    [SerializeField] private Transform anchor_A;
    [SerializeField] private Transform anchor_B;

    private GameObject xrOrigin;
    private RoomVariant currentVariant = RoomVariant.A;

    private void OnEnable()
    {
        xrOrigin = this.GameObject();
        InputHandler.Ins.OnRoomStateSwitch += SwitchRoom;
        
        xrOrigin.transform.position = anchor_A.position;
    }

    private void OnDisable()
    {
        InputHandler.Ins.OnRoomStateSwitch -= SwitchRoom;
    }

    private void SwitchRoom()
    {
        if (currentVariant == RoomVariant.A)
        {
            xrOrigin.transform.position = anchor_A.position;
            currentVariant = RoomVariant.B;
        }
        else
        {
            xrOrigin.transform.position = anchor_B.position;
            currentVariant = RoomVariant.A;
        }
    }
}
