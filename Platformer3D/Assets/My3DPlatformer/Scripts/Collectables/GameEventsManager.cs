using System;
using UnityEngine;

namespace Platformer
{
    public class GameEventsManager : MonoBehaviour
    {
        public event Action onEnemyDeath;
        public event Action onPlayerDeath;
        
        public event Action onCoinCollected;
        public event Action onEcliptiumCollected;
        public event Action onLuminCollected;
        public static GameEventsManager instance { get; private set; }

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Found more than one Game Events Manager in the scene.");
            }

            instance = this;
        }
        public void PlayerDeath()
        {
            if (onPlayerDeath != null)
            {
                onPlayerDeath();
            }
        }
        public void EnemyDeath()
        {
            if (onEnemyDeath != null)
            {
                onEnemyDeath();
            }
        }
        public void CoinCollected()
        {
            if (onCoinCollected != null)
            {
                onCoinCollected();
            }
        }
        
        public void EcliptiumCollected()
        {
            if (onEcliptiumCollected != null)
            {
                onEcliptiumCollected();
            }
        }
        
        public void LuminCollected()
        {
            if (onLuminCollected != null)
            {
                onLuminCollected();
            }
        }
    }
}