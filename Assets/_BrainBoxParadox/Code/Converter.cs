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
    
    
    [Header("Converter Graphics Related")]
    [SerializeField] private GameObject boxModel;
    [SerializeField] private Material[] iconMaterials;


    private void InitializeGraphics()
    {
        var meshRenderer = boxModel.GetComponent<MeshRenderer>();
        meshRenderer.material = iconMaterials[convertableID];
    }
    
    private void Start()
    {
        snapPoint = GetComponent<SnapPoint>();
        InitializeGraphics();
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
        morphHandler = snapPoint.GetOccupyingObject()?.GetComponent<MorphHandler>();
        if (!morphHandler) return;
        
        var currentShapeID = morphHandler.GetShapeID();
        
        if (currentShapeID == 0)
        {
            InputHandler.Ins.morphSliderValue = convertableID;
            morphHandler.UnlockMorphableObject();
        }
        else if (currentShapeID == convertableID)
        {
            if (currentShapeID == 1)
            {
                morphHandler.UnlockMorphableObject();
            }
            else
            {
                morphHandler.ChangeShape(0);
            }
        }
    }
    
}
