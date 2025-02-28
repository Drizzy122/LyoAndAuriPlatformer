using UnityEngine;

namespace Platformer
{
    public class TriggerSave : MonoBehaviour
    {
        private DataPersistenceManager dataPersistenceManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            dataPersistenceManager = FindFirstObjectByType<DataPersistenceManager>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                dataPersistenceManager.SaveGame();
                print("Saved");
            }
        }
    }
}
