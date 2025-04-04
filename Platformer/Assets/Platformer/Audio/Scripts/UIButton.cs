using UnityEngine;
using FMODUnity;
namespace Platformer
{

    public class UIButton : MonoBehaviour
    {
        private StudioEventEmitter emitter;
       [SerializeField] private EventReference buttonEvent;
       public bool PlayOnAwake;
        public void PlayOneShot()
        {
            AudioManager.instance.PlayOneShot(buttonEvent, transform.position);
        }

        private void Start()
        {
            emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.ui, this.gameObject);
            if(PlayOnAwake)
                PlayOneShot();
        }
        
    
    }
}
