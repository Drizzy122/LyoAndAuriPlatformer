using UnityEngine;
using DG.Tweening;

namespace Platformer
{
    public class MenuAnimator : MonoBehaviour
    {
        [SerializeField] RectTransform optionsPanelRect;
        [SerializeField] float topPosY, middlePosY;
        [SerializeField] float tweenDuration;
        
        public void OnEnable()
        {
            Intro();
        }

        public void OnDisable()
        {
            Outro();
        }
        
        void Intro()
        {
            optionsPanelRect.DOAnchorPosY(middlePosY, tweenDuration).SetUpdate(true);
        }

        
        void Outro()
        {
            optionsPanelRect.DOAnchorPosY(topPosY, tweenDuration).SetUpdate(true);
        }
        
        private void OnDestroy()
        {
            DOTween.Kill(optionsPanelRect); // Ensure all tweens for this RectTransform are killed
        }

    }
}