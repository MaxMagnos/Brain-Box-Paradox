using System;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting; // Make sure you have DOTween imported into your project

public class PopEffect : MonoBehaviour
{
    [Header("Pop-In Settings")]
    [Tooltip("The scale the object will animate FROM when enabled.")]
    private Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f); // Starts invisible/very small
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

    public void DestroyWithPop()
    {
        transform.DOScale(startScale, animationDurationOut)
            .SetEase(easeTypeOut)
            .OnComplete(() =>
            {
                // Optional: If you want to ensure the scale is reset after disable,
                // though OnEnable will set it back. Useful if the object
                // might be re-enabled without being fully scaled down.
                transform.localScale = startScale;
                Destroy(this.GameObject());
            });
    }
    


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.DisableWithPop();
        }
    }
}