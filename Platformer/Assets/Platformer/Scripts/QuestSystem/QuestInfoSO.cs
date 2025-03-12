using UnityEngine;

namespace Platformer
{
    [CreateAssetMenu(fileName = "QuestInfoSO", menuName = "Platformer/QuestInfoSO")]
    public class QuestInfoSO : ScriptableObject
    {
        [field: SerializeField] public string id { get; private set; }
        
        [Header("General")]
        public string displayName;
        
        [Header("Requirements")]
        public int levelRequirement;
        public QuestInfoSO[] questPrerequisites;
        
        [Header("Steps")]
        public GameObject[] questStepPrefab;
        
        [Header("Rewards")]
        public int goldReward;
        public int experienceReward;
        public int itemRewards;
        
        // ensure the id is always the name of the scriptable pbject assset
        private void OnValidate()
        {
            #if UNITY_EDITOR
            id = this.name;
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
    }
}