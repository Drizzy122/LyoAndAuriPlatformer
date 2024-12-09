using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Platformer
{
    public class LuminOrbsCollectedText : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private int totalLumins = 0;

        private int luminCollected = 0;
        
        private TextMeshProUGUI luminCollectedText;
        
        private void Awake()
        {
            luminCollectedText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            GameEventsManager.instance.onLuminCollected += OnLuminCollected;
        }
        
        public void LoadData(GameData data)
        {
            foreach (KeyValuePair<string, bool> pair in data.luminCollected)
            {
                if (pair.Value)
                {
                    luminCollected++;
                }
            }
        }
        public void SaveData(GameData data)
        {
            // no data needs to be saved for this script
        }
        
        private void OnDestroy()
        {
            // unsubscribe from events
            GameEventsManager.instance.onCoinCollected -= OnLuminCollected;
        }
        
        private void OnLuminCollected()
        {
            luminCollected++;
        }
        private void Update()
        {
            luminCollectedText.text = luminCollected + " / " + totalLumins;
        }
    }
}
