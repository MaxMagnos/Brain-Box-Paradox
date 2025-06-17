using System;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Makes GameObject float up and down smoothly.
/// Changes to variables are NOT possible at Runtime.
/// </summary>
public class FloatEffect : MonoBehaviour
{
    [Tooltip("The total vertical distance the object will float up and down.")]
    [SerializeField] private float floatDistance = 1.0f;

    [Tooltip("The time it takes to move from the bottom to the top.")]
    [SerializeField] private float floatDuration = 2.0f;

    [Tooltip("The easing function to use for the floating motion. Default is 'InOutSine'")]
    [SerializeField] private Ease easeType = Ease.InOutSine;
    
    private void Start()
    {
        transform.DOMoveY(transform.position.y + floatDistance, floatDuration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
}
