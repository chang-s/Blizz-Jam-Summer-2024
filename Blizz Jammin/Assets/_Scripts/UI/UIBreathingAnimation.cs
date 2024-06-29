using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIBreathingAnimation : MonoBehaviour
{
    public RectTransform uiElement;
    private float breathingScale = 1.05f;
    private float animationDuration = 1.2f;
    [SerializeField] private bool animateOnHover = false;
    private Sequence breathingSequence;

    void Start()
    {
        if (uiElement == null)
        {
            Debug.LogError("UI element is not assigned!");
            return;
        }

        // Scale up animation
        breathingSequence = DOTween.Sequence();
        breathingSequence.Append(uiElement.DOScale(breathingScale, animationDuration / 2))
                        .Append(uiElement.DOScale(1f, animationDuration / 2))
                        .SetLoops(-1); // SetLoops(-1) to loop indefinitely

        // Check if animation should play on hover
        if (animateOnHover)
        {
            DisableAnimation();
        }
    }

    void OnEnable()
    {
        // Subscribe to events for mouse enter and exit
        if (uiElement != null)
        {
            uiElement.gameObject.AddComponent<EventTrigger>().triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerEnter, () => EnableAnimation()));
            uiElement.gameObject.AddComponent<EventTrigger>().triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerExit, () => DisableAnimation()));
        }
    }

    void OnDisable()
    {
        // Unsubscribe from events
        if (uiElement != null)
        {
            Destroy(uiElement.GetComponent<EventTrigger>());
            breathingSequence?.Kill(); // Stop the animation if it's running
        }
    }

    private EventTrigger.Entry CreateEventTriggerEntry(EventTriggerType eventType, UnityEngine.Events.UnityAction callback)
    {
        var entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((eventData) => callback());
        return entry;
    }

    private void EnableAnimation()
    {
        if (breathingSequence != null && !breathingSequence.IsPlaying())
        {
            breathingSequence.Play();
        }
    }

    private void DisableAnimation()
    {
        if (breathingSequence != null && breathingSequence.IsPlaying())
        {
            breathingSequence.Pause();
        }
    }
}
