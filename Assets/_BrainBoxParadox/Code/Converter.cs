using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Converter : MonoBehaviour
{
    [SerializeField] private SnapPoint snapPoint;
    [SerializeField] private MorphHandler morphHandler;
    
    private void Start()
    {
        snapPoint = GetComponent<SnapPoint>();
    }

    private void OnEnable()
    {
        InputHandler.Ins.OnKnock += Convert;
    }
    private void OnDisable()
    {
        InputHandler.Ins.OnKnock -= Convert;
    }

    private void Convert()
    {
        morphHandler = snapPoint.GetOccupyingObject().GetComponent<MorphHandler>();

        if (!morphHandler) return;

        if (morphHandler.GetShapeID() == 0)
        {
            morphHandler.UnlockLightOrb();
        }
        else
        {
            morphHandler.ChangeShape(0);
        }
    }
    
}
