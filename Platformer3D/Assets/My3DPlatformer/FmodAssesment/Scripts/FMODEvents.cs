using UnityEngine;
using FMODUnity;

namespace Platformer
{
    public class FMODEvents : MonoBehaviour
    {
        [field: Header("Player SFX")]
        [field: SerializeField] public EventReference playerFootsteps { get; private set; }
        
        [field: Header("Coin SFX")]
        [field: SerializeField] public EventReference coinCollected { get; private set;}
        public static FMODEvents instance { get; private set; }

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Found more than one FMOD Events instance in the scene");   
            }
            instance = this;
        }
    }
}
