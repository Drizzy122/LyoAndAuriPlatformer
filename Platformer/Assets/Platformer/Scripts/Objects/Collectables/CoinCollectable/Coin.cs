using UnityEngine;
using FMODUnity;

namespace Platformer
{ 
    public class Coin : MonoBehaviour, IDataPersistence
    {
        [Header("Config")]
        [SerializeField] private string id;
        [SerializeField] private bool collected = false;
        [ContextMenu("Generate guid for id")]

        void Awake()
        {
            GenerateGuid();
        }
        private void GenerateGuid()
        {
            id = System.Guid.NewGuid().ToString();
        }
        public void LoadData(GameData data)
        {
            data.coinsCollected.TryGetValue(id, out collected);
            if (collected)
            {
                gameObject.SetActive(false);
            }
        }
        public void SaveData(GameData data)
        {
            if (data.coinsCollected.ContainsKey(id))
            {
                data.coinsCollected.Remove(id);
            }
            data.coinsCollected.Add(id, collected);
        }
        private void CollectCoin()
        {
            collected = true;
            gameObject.SetActive(false);
            GameEventsManager.instance.miscEvents.CoinCollected();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.coinCollected, this.transform.position);
            
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CollectCoin();
            }
        }
    }
}