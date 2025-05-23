using UnityEngine;
using TMPro;

namespace Platformer
{
    public class DeathCountText : MonoBehaviour, IDataPersistence
    {
        private int deathCount = 0;

        private TextMeshProUGUI deathCountText;

        private void Awake()
        {
            deathCountText = this.GetComponent<TextMeshProUGUI>();
        }

        public void LoadData(GameData data)
        {
            this.deathCount = data.deathCount;
        }

        public void SaveData(GameData data)
        {
            data.deathCount = this.deathCount;
        }

        private void Start()
        {
            // subscribe to events
            //GameEventsManager.instance.onPlayerDeath += OnPlayerDeath;
            GameEventsManager.instance.enemyEvents.onEnemyDeath += OnEnemyDeath;
        }

        private void OnDestroy()
        {
            // unsubscribe from events
            ///GameEventsManager.instance.onPlayerDeath -= OnPlayerDeath;
            GameEventsManager.instance.enemyEvents.onEnemyDeath -= OnEnemyDeath;
        }

        private void OnPlayerDeath()
        {
            deathCount++;
        }

        private void OnEnemyDeath()
        {
            deathCount++;
        }

        private void Update()
        {
            deathCountText.text = "" + deathCount;
        }
    }
}