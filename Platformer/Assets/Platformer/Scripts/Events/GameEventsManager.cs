using System;
using UnityEngine;

namespace Platformer
{
    public class GameEventsManager : MonoBehaviour
    {
        public static GameEventsManager instance { get; private set; }
        
        public MiscEvents miscEvents;
        public PlayerEvents playerEvents;
        
        public event Action onEnemyDeath;

        

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Found more than one Game Events Manager in the scene.");
            }

            instance = this;
            
            
            miscEvents = new MiscEvents();
            playerEvents = new PlayerEvents();
        }
        
        // lines of code below needs to be moved
        
        
        
      

       
        
        public void EnemyDeath()
        {
            if (onEnemyDeath != null)
            {
                onEnemyDeath();
            }
        }
    }
}