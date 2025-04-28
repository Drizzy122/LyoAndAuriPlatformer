using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Platformer
{
    public class ButtonAnimator : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public float scaleFactor = 1.2f; // Scale size when selected
        public float duration = 0.2f;   // Animation duration
        private Vector3 originalScale; // Store the initial scale of the button

        private void Awake()
        {
            // Record the original scale of the button
            originalScale = transform.localScale;
        }

        // Triggered when the button gets selected
        public void OnSelect(BaseEventData eventData)
        {
            // Scale up the button
            transform.DOScale(originalScale * scaleFactor, duration).SetEase(Ease.OutBack).SetUpdate(true);
        }

        // Triggered when the button is deselected
        public void OnDeselect(BaseEventData eventData)
        {
            transform.DOKill(); 

            // Scale back to the original size
            transform.DOScale(originalScale, duration).SetEase(Ease.InOutCubic).SetUpdate(true);
        }
        
        private void OnDestroy()
        {
            DOTween.Kill(transform); // Kill all tweens for this transform
        }
    }
}