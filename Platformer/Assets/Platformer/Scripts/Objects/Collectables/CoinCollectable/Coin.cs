using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{ 
    public class Coin : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid() 
        {
            id = System.Guid.NewGuid().ToString();
        }
        
        private MeshRenderer visual;
        private ParticleSystem collectParticle;
        private bool collected = false;
        
        private void Awake() 
        {
            visual = this.GetComponentInChildren<MeshRenderer>();
            collectParticle = this.GetComponentInChildren<ParticleSystem>();
            collectParticle.Stop();
        }
        
        public void LoadData(GameData data) 
        {
            data.coinsCollected.TryGetValue(id, out collected);
            if (collected) 
            {
                visual.gameObject.SetActive(false);
                Destroy(gameObject, 0.5f);
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
       
        private void OnTriggerEnter()
        {
            if (!collected)
            {
                collectParticle.Play();
                CollectCoin();
            }
        }
        private void CollectCoin()
        {
            collected = true;
            visual.gameObject.SetActive(false);
            GameEventsManager.instance.miscEvents.CoinCollected();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.coinCollected, this.transform.position);
            
        }
        
        private void Destroy() 
        {
            Destroy(this.gameObject);
        }
    }
}