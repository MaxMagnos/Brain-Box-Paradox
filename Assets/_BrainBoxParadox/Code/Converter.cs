using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Converter : MonoBehaviour
{
    [SerializeField] private SnapPoint snapPoint;
    [SerializeField] private MorphHandler morphHandler;

    [Header("Designer Variables")]
    [Tooltip("Defines which object can be converted based on ID")]
    [SerializeField] private int convertableID;
    
    
    
    private void Start()
    {
        snapPoint = GetComponent<SnapPoint>();
    }

    private void OnEnable()
    {
        InputHandler.Ins.OnConverterShake += Convert;
    }
    private void OnDisable()
    {
        InputHandler.Ins.OnConverterShake -= Convert;
    }

    private void Convert()
    {
        morphHandler = snapPoint.GetOccupyingObject().GetComponent<MorphHandler>();
        
        if (!morphHandler) return;
        var currentShapeID = morphHandler.GetShapeID();
        if (currentShapeID != convertableID) return;

        if (currentShapeID <= 1)
        {
            morphHandler.UnlockMorphableObject();
        }
        else
        {
            morphHandler.ChangeShape(0);
        }
    }
    
}
