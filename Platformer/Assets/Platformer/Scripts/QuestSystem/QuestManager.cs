using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class QuestManager : MonoBehaviour
    {
        private Dictionary<string, Quest> questMap;
        
        private void Awake()
        {
            questMap = CreateQuestMap();
            Quest quest = GetQuestById("CollectCoinsQuest");
            Debug.Log(quest.info.displayName);
            Debug.Log(quest.info.levelRequirement);
            Debug.Log(quest.state);
            Debug.Log(quest.CurrentStepExist());
        }
        private Dictionary<string,Quest> CreateQuestMap()
        {
            // Loads all QuestInfoSO Scriptable Object under the Assets/Resources/Quest Folder
            QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
            // Creates the quests map
            Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
            foreach (QuestInfoSO questInfo in allQuests)
            {
                if (idToQuestMap.ContainsKey(questInfo.id))
                {
                    Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
                }
                idToQuestMap.Add(questInfo.id, new Quest(questInfo));
            }
            return idToQuestMap;
        }

        private Quest GetQuestById(string id)
        {
            Quest quest = questMap[id];
            if (quest == null)
            {
                Debug.LogError("ID not found in the Quest Map:" + id);
            }
            return quest;
        }
    }
}