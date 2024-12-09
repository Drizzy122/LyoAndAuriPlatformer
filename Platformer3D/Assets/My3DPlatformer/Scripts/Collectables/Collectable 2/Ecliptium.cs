using UnityEngine;

namespace Platformer
{
    public class Ecliptium : MonoBehaviour, IDataPersistence
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
            data.ecliptiumCollected.TryGetValue(id, out collected);
            if (collected)
            {
                gameObject.SetActive(false);
            }
        }
        public void SaveData(GameData data)
        {
            if(data.ecliptiumCollected.ContainsKey(id))
            {
                data.ecliptiumCollected.Remove(id);
            }
            data.ecliptiumCollected.Add(id, collected);
        }
        private void OnTriggerEnter()
        {
            if (!collected)
            {
                CollectEcliptium();
            }
        }
        private void CollectEcliptium()
        {
            collected = true;
            gameObject.SetActive(false);
            GameEventsManager.instance.EcliptiumCollected();
        }
    }
}