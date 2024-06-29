using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIBreathingAnimation : MonoBehaviour
{
    public RectTransform uiElement;
    private float breathingScale = 1.05f;
    private float animationDuration = 1.2f;

    void Start()
    {
        if (uiElement == null)
        {
            Debug.LogError("UI element is not assigned!");
            return;
        }

        // Scale up animation
        Sequence breathingSequence = DOTween.Sequence();
        breathingSequence.Append(uiElement.DOScale(breathingScale, animationDuration / 2))
                        .Append(uiElement.DOScale(1f, animationDuration / 2))
                        .SetLoops(-1); // SetLoops(-1) to loop indefinitely

        // Optionally, you can add ease types (e.g., .SetEase(Ease.InOutQuad))

        // Play the animation
        breathingSequence.Play();
    }
}