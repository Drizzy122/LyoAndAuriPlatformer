using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

namespace Platformer
{
    public class UIButton : MonoBehaviour, ISelectHandler
    {
        private StudioEventEmitter emitter;
        [SerializeField] private EventReference buttonEvent;
        [SerializeField] private EventReference hoverEvent; // New event for hover sound
       
        private void Start()
        {
            // Setup emitter if required
            emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.ui, this.gameObject);
        }

        // Method to play button one-shot event
        public void PlayOneShot()
        {
            AudioManager.instance.PlayOneShot(buttonEvent, transform.position);
        }

        // Method called when the button is selected (highlighted)
        public void OnSelect(BaseEventData eventData)
        {
            // Play hover audio
            AudioManager.instance.PlayOneShot(hoverEvent, transform.position);
        }
    }
}