using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(SphereCollider))]
    public class QuestPoint : MonoBehaviour
    {
        [Header( "Quest" )]
        [SerializeField] private QuestInfoSO questInfoForPoint;
        
        [Header("Configuration")]
        [SerializeField] private bool startPoint = true;
        [SerializeField] private bool finishPoint = true;

        private bool playerIsNear = false;
        private string questId;
        private QuestState currentQuestState;
        private QuestIcon questIcon;

        private void Awake()
        {
            questId = questInfoForPoint.id;
            questIcon = GetComponentInChildren<QuestIcon>();
        }

        private void OnEnable()
        {
            GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
            //TODO - Find a way to interact with the collider like press e to interact
        }

        private void OnDisable()
        {
            GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        }

        private void SubmitPressed()
        {
            if (!playerIsNear)
            {
                return;
            }
            if(currentQuestState.Equals(QuestState.CAN_START) && startPoint)
            {
                GameEventsManager.instance.questEvents.StartQuest(questId);
            }
            else if(currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
            {
                GameEventsManager.instance.questEvents.FinishQuest(questId);
            }
        }
        private void QuestStateChange(Quest quest)
        {
           // only update the quest state if this point has the corresponding quest
           if (quest.info.id.Equals(questId))
           {
               currentQuestState = quest.state;
               questIcon.SetState(currentQuestState, startPoint, finishPoint);
           }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerIsNear = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerIsNear = false;
            }
        }
    }
}