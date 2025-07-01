using System;
using Unity.VisualScripting;
using UnityEngine;


public class MorphHandler : MonoBehaviour
{
    [SerializeField] private Grabable grabable;
    
    [SerializeField] public GameObject[] shapes;
    [SerializeField] private int currentShapeID;
    [SerializeField] private int lockCount;

    private void Awake()
    {
        grabable = GetComponent<Grabable>();
        UpdateShape();
    }

    private void OnEnable()
    {
        grabable.OnGrabbed += HandleGrab;
        grabable.OnDropped += HandleDrop;
        
        InputHandler.Ins.OnMorphSliderChange += HandleSliderChange;
    }

    private void OnDisable()
    {
        grabable.OnGrabbed -= HandleGrab;
        grabable.OnDropped -= HandleDrop;
        
        InputHandler.Ins.OnMorphSliderChange -= HandleSliderChange;
    }

    /// <summary>
    /// Checks if Shape-Change is allowed and sets new currentShapeID if so. Also calls UpdateShape()
    /// </summary>
    /// <param name="newShapeID"></param>
    public void ChangeShape(int newShapeID)
    {
        if (lockCount > 0 || newShapeID == currentShapeID) return;
        if (newShapeID >= shapes.Length)
        {
            Debug.LogWarning("MorphHandler: Shape ID is out of range. Falling back to default.");
            newShapeID = 1;
        }
        
        currentShapeID = newShapeID;
        
        //Lock the Object if it is changed to the LightOrb
        if (currentShapeID == 0)
        {
            lockCount++;
        }
        
        UpdateShape();
    }

    public int GetShapeID()
    {
        return currentShapeID;
    }


    private void UpdateShape()
    {
        for (int i = 0; i < shapes.Length; i++)
        {
            shapes[i].SetActive(i == currentShapeID);
        }
    }

    public void UnlockLightOrb()
    {
        if (currentShapeID == 0 && lockCount > 0)
        {
            lockCount--;
            ChangeShape(InputHandler.Ins.morphSliderValue);
        }
    }

    private void HandleSliderChange()   //TODO: Refactor this method so that the SliderChange event in the InputHandler carries it's value as a parameter
    {
        ChangeShape(InputHandler.Ins.morphSliderValue);
    }
    
    private void HandleGrab()
    {
        lockCount++;
    }

    private void HandleDrop()
    {
        lockCount--;
    }
}
