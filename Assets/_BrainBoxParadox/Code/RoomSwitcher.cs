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
    [SerializeField] private Vector3 anchor_A;
    [SerializeField] private Vector3 anchor_B;

    private GameObject xrOrigin;
    private RoomVariant currentVariant = RoomVariant.A;

    private void OnEnable()
    {
        xrOrigin = this.GameObject();
        InputHandler.Ins.OnRoomStateSwitch += SwitchRoom;
    }

    private void OnDisable()
    {
        InputHandler.Ins.OnRoomStateSwitch -= SwitchRoom;
    }

    private void SwitchRoom()
    {
        if (currentVariant == RoomVariant.A)
        {
            xrOrigin.transform.position = anchor_A;
            currentVariant = RoomVariant.B;
        }
        else
        {
            xrOrigin.transform.position = anchor_B;
            currentVariant = RoomVariant.A;
        }
    }

    public void SetAnchors(Vector3 newAnchorA, Vector3? newAnchorB = null)
    {
        anchor_A = newAnchorA;

        if (!newAnchorB.HasValue)
        {
            anchor_B = anchor_A;      //Fallback: If there is no Anchor B then both anchors will bet set to A's position
        }
        else
        {
            anchor_B = newAnchorB.Value;
        }

        currentVariant = RoomVariant.A;
        SwitchRoom();
    }
}
