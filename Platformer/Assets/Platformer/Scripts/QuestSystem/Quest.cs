using System.Collections;
using UnityEngine;

namespace Platformer
{
    public class Quest 
    {
        // static info
        public QuestInfoSO info;

        // state info
        public QuestState state;

        private int currentQuestStepIndex;
        
        private QuestStepState[] questStepStates;
        public Quest(QuestInfoSO questInfo)
        {
            this.info = questInfo;
            this.state = QuestState.REQUIREMENT_NOT_MET;
            this.currentQuestStepIndex = 0;
            this.questStepStates = new QuestStepState[info.questStepPrefab.Length];
            for (int i = 0; i < questStepStates.Length; i++)
            {
                questStepStates[i] = new QuestStepState();
            }
        }

        public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
        {
            this.info = questInfo;
            this.state = questState;
            this.currentQuestStepIndex = currentQuestStepIndex;
            this.questStepStates = questStepStates;
            
            // if the quest step state and prefabs are different leghts,
            // something has changed during development and the save data is out of sync.
            if (this.questStepStates.Length != this.info.questStepPrefab.Length)
            {
                Debug.LogError("Quest step prefabs and quest step state are"
                               + "of different leghts. this indicates something changed "
                               + "with the questInfo and the save data is now out of sync."
                               + "Reset your data- as this might cause issues. QuestId: " + this.info.id);
            }
        }
        public void MoveToNextStep()
        {
            currentQuestStepIndex++;
        }

        public bool CurrentStepExist()
        {
            return (currentQuestStepIndex < info.questStepPrefab.Length);
        }

        public void InstantiateCurrentQuestStep(Transform parentTransform)
        {
            GameObject questStepPrefab = GetCurrentQuestStepPrefab();
            if (questStepPrefab != null)
            {
                QuestStep questStep = Object.Instantiate<GameObject>(questStepPrefab,parentTransform).GetComponent<QuestStep>();
                questStep.InitializeQuestStep(info.id , currentQuestStepIndex, questStepStates[currentQuestStepIndex].state);
            }
        }
        
        private GameObject GetCurrentQuestStepPrefab()
        {
            GameObject questStepPrefab = null;
            if (CurrentStepExist())
            {
                questStepPrefab = info.questStepPrefab[currentQuestStepIndex];
            }
            else
            {
                Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that "
                    + "there's no current step: QuestId=" + info.id + ", stepIndex=" + currentQuestStepIndex);
            }
            return questStepPrefab;
        }
        
        public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
        {
            if (stepIndex < questStepStates.Length)
            {
                questStepStates[stepIndex].state = questStepState.state;
            }
            else
            {
                Debug.LogWarning("Tried to access quest step data, but stepIndex was out of range:" 
                                 + "Quest id = "+ info.id + ", step Index = " + stepIndex);
            }
        }

        public QuestData GetQuestData()
        {
            return new QuestData(state, currentQuestStepIndex, questStepStates);
        }
    }
}