using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Platformer
{
    public class EcliptiumCollectedText : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private int totalEcliptium; 
        private int ecliptiumCollected = 0;
        private TextMeshProUGUI ecliptiumCollectedText;
        
        private void Awake()
        {
            ecliptiumCollectedText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            GameEventsManager.instance.miscEvents.onEcliptiumCollected += OnEcliptiumCollected;
        }
        public void LoadData(GameData data)
        {
            foreach (KeyValuePair<string, bool> pair in data.ecliptiumCollected)
            {
                if (pair.Value)
                {
                    ecliptiumCollected++;
                }
            }
        }
        public void SaveData(GameData data)
        {
            // no data needs to be saved for this script
        }
        
        private void OnDestroy()
        {
            GameEventsManager.instance.miscEvents.onEcliptiumCollected -= OnEcliptiumCollected;
        }

        private void OnEcliptiumCollected()
        {
            ecliptiumCollected++;
        }

        private void Update()
        {
            ecliptiumCollectedText.text = ecliptiumCollected + " / " + totalEcliptium;
        }
    }
}
