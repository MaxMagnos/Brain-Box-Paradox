using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class MorphHandler : MonoBehaviour
{
    private Grabable grabable;
    
    [SerializeField] public GameObject[] shapes;
    [SerializeField] private int currentShapeID;
    [SerializeField] public int lockCount;

    private void Awake()
    {
        grabable = GetComponent<Grabable>();
        ChangeShape(currentShapeID);
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
        //if (lockCount > 0 || newShapeID == currentShapeID) return;
        if (lockCount > 0) return;
        if (newShapeID >= shapes.Length)
        {
            Debug.LogWarning("MorphHandler: Shape ID is out of range. Falling back to default.");
            newShapeID = 1;
        }
        
        currentShapeID = newShapeID;
        
        //Lock the Object if it is changed to the LightOrb or Off-Lightbulb
        if (currentShapeID <= 1)
        {
            lockCount++;
        }
        
        Debug.Log("ChangeShape ran. Calling UpdateShape() with curShapeID: " + currentShapeID);
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

    public void UnlockMorphableObject()
    {
        if (lockCount <= 0) return;

        lockCount--;
        if (currentShapeID == 0)
        {
            ChangeShape(InputHandler.Ins.morphSliderValue);
        }
        else if (currentShapeID == 1)
        {
            ChangeShape(0);
        }
    }

    public void TurnOnLightbulb()   //TODO: Refactor this method because it sucks ass but i can't be bothered to find a more clever solution for the "UnlockMorphableObject" one.
    {
        lockCount = 0;
        ChangeShape(2);
    }

    private void HandleSliderChange()   //TODO: Refactor this method so that the SliderChange event in the InputHandler carries it's value as a parameter
    {
        ChangeShape(InputHandler.Ins.morphSliderValue +1);
    }
    
    private void HandleGrab()
    {
        lockCount++;
        
        char sfxVariation = char.Parse(Random.Range(1, 4).ToString());
        string sfx = "SwitchClick_V1";
        switch (currentShapeID)
        {
            case 0:
                sfx = "LightOrbGrab_V" + sfxVariation;
                break;
            
            case 1 or 2:
                sfx = "LightbulbGrab_V" + sfxVariation;
                break;
            
            case 3:
                sfx = "EyeGrab_V" + sfxVariation;
                break;
            
            case 4:
                sfx = "ScissorGrab_V" + sfxVariation;
                break;
            
            default: break;
        }
        AudioManager.Instance.PlaySound(sfx, transform.position, 1f);
    }

    private void HandleDrop()
    {
        lockCount--;
    }
}
