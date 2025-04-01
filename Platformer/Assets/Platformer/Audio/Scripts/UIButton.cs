using UnityEngine;
using FMODUnity;

namespace Platformer
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class UIButton : MonoBehaviour
    {
        private StudioEventEmitter emitter;
        private void Start()
        {
            emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.ui, this.gameObject);
        }
    }
}
