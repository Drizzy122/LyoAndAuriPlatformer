using UnityEngine;
using DG.Tweening;
public class MoveObjectUpAndDown : MonoBehaviour
{
    public float moveDistance = 3f; // How far up/down the object will move
    public float moveDuration = 2f; // Duration for one complete up/down movement

    private void Start()
    {
        // Start the move animation
        MoveUpAndDown();
    }

    private void MoveUpAndDown()
    {
        // Get current Y position
        float startYPosition = transform.position.y;

        // Move the object up and then down in an infinite loop
        transform.DOMoveY(startYPosition + moveDistance, moveDuration)
            .SetEase(Ease.InOutSine) // Smooth ease for better visuals
            .SetLoops(-1, LoopType.Yoyo); // Infinite number of loops, moving up and down
    }
}