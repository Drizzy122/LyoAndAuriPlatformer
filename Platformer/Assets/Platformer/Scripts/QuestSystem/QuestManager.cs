using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class QuestManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool loadQuestState = true;
        private Dictionary<string, Quest> questMap;
        
        // quest start requirements
        private int currentPlayerLevel;
        
        private void Awake()
        {
            questMap = CreateQuestMap();
        }

        private void onEnable()
        {
            GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
            GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
            GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;
            
            GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;
            
            GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
        }

        private void onDisable()
        {
            GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
            GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
            GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;
            
            GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
            
            GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
        }

        private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
        {
            Quest quest = GetQuestById(id);
            quest.StoreQuestStepState(questStepState, stepIndex);
            ChangedQuestState(id, quest.state);
        }

        private void Start()
        {
            foreach (Quest quest in questMap.Values)
            {
                if (quest.state == QuestState.IN_PROGRESS)
                {
                    quest.InstantiateCurrentQuestStep(this.transform);
                }
                GameEventsManager.instance.questEvents.QuestStateChange(quest);
            }
        }

        private void ChangedQuestState(string id, QuestState state)
        {
            Quest quest = GetQuestById(id);
            quest.state = state;
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
        
        private void PlayerLevelChange(int level)
        {
            currentPlayerLevel = level;
        }

        private bool CheckRequirementsMet(Quest quest)
        {
            // Start true and prove to be false
          
            bool meetsRequirements = true;
            
            // check player level requirements
            if (currentPlayerLevel < quest.info.levelRequirement)
            {
                meetsRequirements = false;
            }
            // check quest prerequisites for completion
            foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
            {
                if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
                {
                    meetsRequirements = false;
                }
            }
            return meetsRequirements;
        }

        private void Update()
        {
            // loop through All quests
            foreach (Quest quest in questMap.Values)
            {
                // if we're meeting the requirements, switch over the CAN_START state
                if (quest.state == QuestState.REQUIREMENT_NOT_MET && CheckRequirementsMet(quest))
                {
                    ChangedQuestState(quest.info.id, QuestState.CAN_START);
                }
            }
        }
        

        private void StartQuest(string id)
        {
            Quest quest = GetQuestById(id);
            quest.InstantiateCurrentQuestStep(this.transform);
            ChangedQuestState(quest.info.id, QuestState.IN_PROGRESS);
        }
        
        private void AdvanceQuest(string id)
        {
            Quest quest = GetQuestById(id);
            
            // move on to the next step
            quest.MoveToNextStep();
            
            // if there are more steps, instatiate the next one
            if (quest.CurrentStepExist())
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            // if there are no more steps, then we've finished all of them for this quest
            else
            {
                ChangedQuestState(quest.info.id, QuestState.FINISHED);
            }
        }
        
        private void FinishQuest(string id)
        {
            Quest quest = GetQuestById(id);
            ClaimRewards(quest);
            ChangedQuestState(quest.info.id, QuestState.FINISHED);
        }

        private void ClaimRewards(Quest quest)
        {
            //GameEventsManager.instance.goldEvents.GoldGained(quest.info.goldReward); // add a way to give the player some rewards
            GameEventsManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
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
                idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
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

        private void OnApplicationQuit()
        {
            foreach (Quest quest in questMap.Values)
            {
               SaveQuest(quest);
            }
        }

        private void SaveQuest(Quest quest)
        {
            try
            {
                QuestData questData = quest.GetQuestData();
                // serialize using JsonUtility
                string serializedData = JsonUtility.ToJson(questData);
                // saving to playerPref is just an example
                // instead, use the save & load system
                PlayerPrefs.SetString(quest.info.id, serializedData);

                
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to save quest with id" +quest.info.id + " : " + e);
            }
        }

        private Quest LoadQuest(QuestInfoSO questInfo)
        {
            Quest quest = null;
            try
            {
                // load quest from saved data
                if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
                {
                    string serializedData = PlayerPrefs.GetString(questInfo.id);
                    QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                    quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
                }
                // otherwise, initialize a new quest
                else
                {
                    quest = new Quest(questInfo);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load quest with id" + quest.info.id + " : " + e);
            }
            return quest;
        }
    }
}