using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace Platformer
{
    public class SceneFade : MonoBehaviour
    {
        private Image _sceneFadeImage;

        private void Awake()
        {
            _sceneFadeImage = GetComponent<Image>();
        }

        public IEnumerator FadeOutCoroutine(float fadeDuration, Color fadeColor)
        {
            gameObject.SetActive(true);

            Color startColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, _sceneFadeImage.color.a);
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1);

            yield return FadeCoroutine(fadeDuration, startColor, targetColor);
        }

        public IEnumerator FadeInCoroutine(float fadeDuration, Color fadeColor)
        {
            Color startColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1);
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0);

            yield return FadeCoroutine(fadeDuration, startColor, targetColor);

            gameObject.SetActive(false);
        }

        private IEnumerator FadeCoroutine(float fadeDuration, Color startColor, Color targetColor)
        {
            float elapsedTime = 0;
            float elapsedPercentage = 0;
            bool firstFrame = true;

            while (elapsedPercentage < 1)
            {
                elapsedPercentage = elapsedTime / fadeDuration;
                _sceneFadeImage.color = Color.Lerp(startColor, targetColor, elapsedPercentage);
                yield return null;

                if (firstFrame == false)
                {
                    elapsedTime += Time.unscaledDeltaTime;
                }

                firstFrame = false;
            }

            _sceneFadeImage.color = targetColor;
        }
    }
}
