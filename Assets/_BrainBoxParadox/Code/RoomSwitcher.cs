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
    [SerializeField] private GrabHandler grabHandler;
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
        // Store the grabbed object's position RELATIVE TO THE XR ORIGIN'S CURRENT LOCAL SPACE.
        Vector3? grabbedObjectLocalOffset = null; 
        var grabbedObject = grabHandler.GetGrabbedObject();

        if (grabbedObject != null)
        {
            grabbedObjectLocalOffset = xrOrigin.transform.InverseTransformPoint(grabbedObject.transform.position);
        }

        // Determine the NEW target position for the XR Origin based on the current variant.
        // This is the part that needed fixing to ensure bidirectional movement.
        if (currentVariant == RoomVariant.A)
        {
            xrOrigin.transform.position = anchor_B; 
            currentVariant = RoomVariant.B;
        }
        else // currentVariant == RoomVariant.B
        {
            xrOrigin.transform.position = anchor_A; 
            currentVariant = RoomVariant.A;
        }

        // Now, move the grabbed object maintaining its relative position to the new XR Origin position.
        if (grabbedObject != null && grabbedObjectLocalOffset.HasValue)
        {
            grabbedObject.transform.position = xrOrigin.transform.TransformPoint(grabbedObjectLocalOffset.Value);
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

        currentVariant = RoomVariant.B;
        SwitchRoom();
    }
}
