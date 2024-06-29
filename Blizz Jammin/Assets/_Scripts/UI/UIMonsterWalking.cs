using UnityEngine;
using DG.Tweening;

public class UIMonsterWalking : MonoBehaviour
{
    public RectTransform monsterImage;
    private float minWalkingDistance = 0.5f;
    private float maxWalkingDistance = 1f;
    private float minWalkingDuration = 10f;  
    private float maxWalkingDuration = 15f;  
    private float minWiggleAngle = 5f;
    private float maxWiggleAngle = 10f;
    private float minWiggleDuration = 0.5f;
    private float maxWiggleDuration = 1f;

    void Start()
    {
        // Generate random values
        float walkingDistance = Random.Range(minWalkingDistance, maxWalkingDistance);
        float walkingDuration = Random.Range(minWalkingDuration, maxWalkingDuration);
        float wiggleAngle = Random.Range(minWiggleAngle, maxWiggleAngle);
        float wiggleDuration = Random.Range(minWiggleDuration, maxWiggleDuration);

        // Create sequences for walking and wiggling animations
        Sequence walkingSequence = DOTween.Sequence();
        Sequence wiggleSequence = DOTween.Sequence();

        // Walking animation (back and forth along X-axis)
        walkingSequence.Append(monsterImage.DOAnchorPosX(walkingDistance, walkingDuration / 2))
                      .Append(monsterImage.DOAnchorPosX(-walkingDistance, walkingDuration))
                      .Append(monsterImage.DOAnchorPosX(0f, walkingDuration / 2))
                      .SetLoops(-1, LoopType.Restart);

        // Wiggling animation (rotation)
        wiggleSequence.Append(monsterImage.DORotate(new Vector3(0f, 0f, wiggleAngle), wiggleDuration / 2))
                      .Append(monsterImage.DORotate(new Vector3(0f, 0f, -wiggleAngle), wiggleDuration))
                      .Append(monsterImage.DORotate(Vector3.zero, wiggleDuration / 2))
                      .SetLoops(-1, LoopType.Restart);

        // Play both sequences simultaneously
        walkingSequence.Play();
        wiggleSequence.Play();
    }
}
