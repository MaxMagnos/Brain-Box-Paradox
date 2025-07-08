using System;
using System.Collections;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    private AudioSource voiceover;
    
    [Header("SpawningObjects")]
    [SerializeField] private GameObject grabTutorialLight; //Child object of TutorialHandler, manually placed/referenced
    [SerializeField] private GameObject grabTutorialBulb;


    private Grabable lightGrabable;
    private MorphHandler bulbMorphHandler;

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
        
        yield return StartCoroutine(PlayNextLine());

        yield return new WaitForSeconds(0.5f);
        
        grabTutorialBulb.SetActive(true);

        while (bulbMorphHandler.GetShapeID() != 2)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(PlayNextLine());
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
    
    void Awake()
    {
        voiceover = GetComponent<AudioSource>();
        lightGrabable = grabTutorialLight.GetComponent<Grabable>();
        bulbMorphHandler = grabTutorialBulb.GetComponent<MorphHandler>();
        
        grabTutorialBulb.SetActive(false);
        grabTutorialLight.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(StartTutorial());
    }

    void Update()
    {
        
    }
    
}
