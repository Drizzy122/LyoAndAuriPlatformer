using UnityEngine;

namespace Platformer
{
    public class QuestPoint : MonoBehaviour
    {
        [Header("Dialogue (optional)")] 
        [SerializeField] private string dialogueKnotName;
        
        [Header("Quest")]
        [SerializeField] private QuestInfoSO questInfoForPoint;
        
        [Header("Config")]
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
            GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        }

        private void OnDisable()
        {
            GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
            GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        }

        private void SubmitPressed(InputEventContext inputEventContext)
        {
            if (!playerIsNear || !inputEventContext.Equals(InputEventContext.DEFAULT))
            {
                return;
            }
            
            // if we have a knot name defined, try to start dialogue with it
            if (!dialogueKnotName.Equals(""))
            {
                GameEventsManager.instance.dialogueEvents.EnterDialogue(dialogueKnotName);
            }
            // otherwise, start or finish the quest immediatelu without dialogue
            else
            {
                // start or finish quest
                if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
                {
                    GameEventsManager.instance.questEvents.StartQuest(questId);
                }
                else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
                {
                    GameEventsManager.instance.questEvents.FinishQuest(questId);
                }
            }
        }

        private void QuestStateChange(Quest quest)
        {
            if (quest.info.id.Equals(questId))
            {
                currentQuestState = quest.state;
                questIcon.SetState(currentQuestState, startPoint, finishPoint);
            }
        }
        private void OnTriggerEnter(Collider otherCollider)
        {
            if (otherCollider.CompareTag("Player"))
            {
                
                playerIsNear = true;
            }
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            if (otherCollider.CompareTag("Player"))
            {
                playerIsNear = false;
            }
        }
    }
}