using System;
using UnityEngine;
using TMPro;
using DG.Tweening; // Don't forget to add this using directive for DOTween

public class PauseBreatheEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text; // Assign this in the Inspector
    [SerializeField] private float breatheDuration = 1.5f; // Duration for one full breathe cycle (fade out and fade back in)
    [SerializeField] private Ease easeType = Ease.InOutSine; // Public field to choose easing type in Inspector

    private Color originalColor;
    private Tween currentTween; // Store a reference to the active tween

    // Called when the script instance is being loaded.
    // Use Awake for initialization before the game starts.
    private void Awake()
    {
        // If the text is not assigned in the Inspector, try to get it from the GameObject.
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        // Store the original color of the text.
        originalColor = text.color;
    }

    // Called when the object becomes enabled and active.
    private void OnEnable()
    {
        // Ensure the text component is valid before starting the animation.
        if (text != null)
        {
            StartBreatheAnimation();
        }
    }

    // Called when the object becomes disabled or inactive.
    private void OnDisable()
    {
        // Kill any active tween when the object is disabled to prevent errors
        // and ensure the tween doesn't continue running in the background.
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill(); // Kill the tween immediately
            currentTween = null; // Clear the reference
        }

        // Optionally, reset the alpha to original when disabled
        // This prevents the text from being invisible if disabled mid-fade.
        if (text != null)
        {
            Color currentColor = text.color;
            currentColor.a = originalColor.a;
            text.color = currentColor;
        }
    }

    /// <summary>
    /// Starts the breathing animation for the text.
    /// The text will fade out to 0 alpha and then fade back to its original alpha,
    /// repeating endlessly with a Yoyo loop type.
    /// </summary>
    private void StartBreatheAnimation()
    {
        // Ensure any previous tween is killed before starting a new one
        // This is important if OnEnable is called multiple times without OnDisable in between
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // Set the initial alpha to the original color's alpha to ensure it starts visible.
        Color startColor = text.color;
        startColor.a = originalColor.a;
        text.color = startColor;

        // Create the DOTween animation:
        // 1. Fade the text's alpha to 0 (fully transparent).
        // 2. Set the duration of one half-cycle (fade out). The total breatheDuration is for a full yoyo cycle.
        // 3. Set the easing type for the animation.
        // 4. Set the loop type to Yoyo, which means it will go back and forth (fade out then fade in).
        // 5. Set the number of loops to -1 for an endless loop.
        // 6. Use SetLink(gameObject) to automatically kill the tween if the GameObject is destroyed.
        currentTween = text.DOFade(0f, breatheDuration / 2) // Fade out to 0 alpha over half the duration
                           .SetEase(easeType) // Apply the chosen easing
                           .SetLoops(-1, LoopType.Yoyo) // Loop endlessly, going back and forth
                           .SetLink(gameObject); // Link the tween to this GameObject for auto-killing on destroy
    }

    // Update is not needed for this animation as DOTween handles it.
    private void Update()
    {
        // No code needed here, DOTween manages the animation.
    }
}
