using UnityEngine;

namespace Platformer
{
    public class SceneFadingOutEventArgs : MonoBehaviour
    {
        public float FadeDuration { get; private set; }

        public SceneFadingOutEventArgs(float fadeDuration)
        {
            FadeDuration = fadeDuration;
        }
    }
}
