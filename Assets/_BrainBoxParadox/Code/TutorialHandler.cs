using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TutorialHandler : MonoBehaviour
{
    private AudioSource voiceover;
    [SerializeField] private AudioSource successSound;

    [SerializeField] private Volume globalVolume;
    
    [Header("SpawningObjects")]
    [SerializeField] private GameObject grabTutorialLight; //Child object of TutorialHandler, manually placed/referenced
    [SerializeField] private GameObject grabTutorialBulb;
    [SerializeField] private GameObject shakeTutorialBox;
    [SerializeField] private GameObject morphTutorialEye;
    

    private Grabable lightGrabable;
    private MorphHandler bulbMorphHandler;
    private bool roomSwitcheActivated = false;
    private bool shakeActivated = false;
    private bool morphSliderActivated = false;

    public IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(PlayNextLine());
        
        yield return new WaitForSeconds(0.5f);
        
        grabTutorialLight.SetActive(true);
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(PlayNextLine());
        
        while (!lightGrabable.grabbed)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        //Play success sound
        yield return StartCoroutine(PlaySuccessSound());
        
        yield return StartCoroutine(PlayNextLine());    //"Well done! Now, give that light to the light bulb over there."
        
        //Wait for player to put LightOrb into Bulb
        grabTutorialBulb.SetActive(true);
        while (bulbMorphHandler.GetShapeID() != 2)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        //Play success sound
        yield return StartCoroutine(PlaySuccessSound());
        
        grabTutorialBulb.GetComponent<PopEffect>().DestroyWithPop();
        
        yield return StartCoroutine(PlayNextLine());    //"Fantastic, you’re getting better at this! Now, let’s turn off the light for a moment."
        
        
        //Wait for Room Switch Press
        roomSwitcheActivated = false;
        while (roomSwitcheActivated == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        //Turn off Lights
        globalVolume.weight = 0;
        
        //Play success sound
        yield return StartCoroutine(PlaySuccessSound());
        
        yield return StartCoroutine(PlayNextLine());    //"Great, you can turn it back on."
        
        //Wait for Room Switch Press
        roomSwitcheActivated = false;
        while (roomSwitcheActivated == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        //Turn lights back on
        globalVolume.weight = 1;
        
        //Play success sound
        yield return StartCoroutine(PlaySuccessSound());
        
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayNextLine());    //"Okay, two more things to calibrate. First, why don’t you give yourself a little shake?"
        
        //Spawn Box
        shakeTutorialBox.SetActive(true);
        
        //Wait for Shake
        shakeActivated = false;
        while (shakeActivated == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        //Play success sound
        yield return StartCoroutine(PlaySuccessSound());
        
        //Despawn Box and activate Eye at same position
        morphTutorialEye.transform.position = shakeTutorialBox.transform.position;
        shakeTutorialBox.GetComponent<PopEffect>().DestroyWithPop();
        morphTutorialEye.SetActive(true);
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(PlayNextLine());    //"Amazing work! Alright, the last one is simple. Change the picture on the box. " (LINE WAS CHANGED)
        
        //Wait for MorphSliderChange
        morphSliderActivated = false;
        while (morphSliderActivated == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        //Play success sound
        yield return StartCoroutine(PlaySuccessSound());
        
        //Destroy MorphObject
        morphTutorialEye.GetComponent<PopEffect>().DestroyWithPop();
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(PlayNextLine());    //"And that’s all you’ll need! You’re ready to go now. Good luck… I’m sure this’ll go better than last time." 
        
        yield return new WaitForSeconds(2f);
        
        LevelManager.Instance.LoadNextLevel();
    }
    
    private IEnumerator PlayNextLine()
    {
        voiceover.Play();
        while (voiceover.isPlaying)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                voiceover.Stop();
                break;
            }
            yield return null;
        }
    }

    private IEnumerator PlaySuccessSound()
    {
        successSound.Play();
        while (successSound.isPlaying)
        {
            yield return null;
        }
    }

    void Awake()
    {
    }

    private void Start()
    {
        voiceover = GetComponent<AudioSource>();
        lightGrabable = grabTutorialLight.GetComponent<Grabable>();
        bulbMorphHandler = grabTutorialBulb.GetComponent<MorphHandler>();
        
        grabTutorialBulb.SetActive(false);
        grabTutorialLight.SetActive(false);
        shakeTutorialBox.SetActive(false);
        morphTutorialEye.SetActive(false);

        InputHandler.Ins.OnRoomStateSwitch += RoomSwitchActivated;
        InputHandler.Ins.OnConverterShake += ShakeActivated;
        InputHandler.Ins.OnMorphSliderChange += MorphSliderActivated;
        
        
        StartCoroutine(StartTutorial());
    }

    void Update()
    {
        
    }

    private void RoomSwitchActivated()
    {
        roomSwitcheActivated = true;
    }

    private void ShakeActivated()
    {
        shakeActivated = true;
    }

    private void MorphSliderActivated()
    {
        morphSliderActivated = true;
    }
}
