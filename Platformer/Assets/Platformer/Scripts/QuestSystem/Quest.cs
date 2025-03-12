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

        public Quest(QuestInfoSO questInfo)
        {
            this.info = questInfo;
            this.state = QuestState.REQUIREMENT_NOT_MET;
            this.currentQuestStepIndex = 0;
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
                Object.Instantiate<GameObject>(questStepPrefab, parentTransform);
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
    }
}