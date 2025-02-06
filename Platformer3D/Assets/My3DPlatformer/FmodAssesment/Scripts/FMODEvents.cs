using UnityEngine;
using FMODUnity;

namespace Platformer
{
    public class FMODEvents : MonoBehaviour
    {
        [field: Header("Player SFX")]
        [field: SerializeField] public EventReference playerFootsteps { get; private set; }
        [field: SerializeField] public EventReference playerJump { get; private set; }
        [field: SerializeField] public EventReference playerSpin { get; private set; }
       // [field: SerializeField] public EventReference playerAttack { get; private set; }
        [field: SerializeField] public EventReference playerHurt { get; private set; }
        [field: SerializeField] public EventReference playerDeath { get; private set; }
        
        [field:Header("Enemy SFX")]
        [field: SerializeField] public EventReference enemyFootsteps { get; private set; }
        [field: SerializeField] public EventReference enemyAttack { get; private set; }
        [field: SerializeField] public EventReference enemyHurt { get; private set; }
        [field: SerializeField] public EventReference enemyDeath { get; private set; }
        
        
        
        
        
        
        
        
        
        
        [field: Header("Music")] 
        [field: SerializeField] public EventReference music { get; private set; }
        
        [field: Header("Ambience")]
        [field: SerializeField] public EventReference ambience { get; private set; }
        

        
        [field: Header("Coin SFX")]
        [field: SerializeField] public EventReference coinCollected { get; private set;}
        
        [field: SerializeField] public EventReference coinidle { get; private set; }
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