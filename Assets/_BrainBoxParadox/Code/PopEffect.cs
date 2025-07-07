using System;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting; // Make sure you have DOTween imported into your project

public class PopEffect : MonoBehaviour
{
    [Header("Pop-In Settings")]
    [Tooltip("The scale the object will animate FROM when enabled.")]
    [SerializeField] private Vector3 startScale = Vector3.zero; // Starts invisible/very small
    [Tooltip("The scale the object will animate TO when enabled.")]
    [SerializeField] private Vector3 endScale = Vector3.one; // Ends at its normal size

    [Header("Animation Properties")]
    [Tooltip("Duration of the pop animation in seconds.")]
    [SerializeField] private float animationDurationIn = 0.3f;
    [Tooltip("Ease type for the animation (e.g., Ease.OutBack for a bouncy pop).")]
    [SerializeField] private Ease easeTypeIn = Ease.OutBack;
    
    [Tooltip("Duration of the pop animation in seconds.")]
    [SerializeField] private float animationDurationOut = 0.3f;
    [Tooltip("Ease type for the animation (e.g., Ease.OutBack for a bouncy pop).")]
    [SerializeField] private Ease easeTypeOut = Ease.OutBack;

    private void OnEnable()
    {
        // Ensure the object starts at the specified start scale before animating
        transform.localScale = startScale;

        // Animate from startScale to endScale
        transform.DOScale(endScale, animationDurationIn)
                 .SetEase(easeTypeIn);
    }

    private void OnDisable()
    {
    }

    public void DisableWithPop()
    {
        transform.DOScale(startScale, animationDurationOut)
            .SetEase(easeTypeOut)
            .OnComplete(() =>
            {
                // Optional: If you want to ensure the scale is reset after disable,
                // though OnEnable will set it back. Useful if the object
                // might be re-enabled without being fully scaled down.
                transform.localScale = startScale;
                this.GameObject().SetActive(false);
            });
    }

    // Optional: Reset scale in editor for easier placement if needed
    private void OnValidate()
    {
        // Only apply if in editor and not playing, to help with setup
        if (Application.isEditor && !Application.isPlaying)
        {
            if (endScale == Vector3.zero) endScale = Vector3.one; // Default to normal size
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.DisableWithPop();
        }
    }
}