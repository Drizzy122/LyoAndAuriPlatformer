using UnityEngine;

namespace Platformer
{
    public class LuminOrbs : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private string id;
        [SerializeField] private bool collected = false;

        [ContextMenu("Generate guid for id")]

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

        private void OnTriggerEnter()
        {
            if (!collected)
            {
                LuminCollected();
            }
        }

        private void LuminCollected()
        {
            collected = true;
            gameObject.SetActive(false);
            GameEventsManager.instance.LuminCollected();
        }
    }
}