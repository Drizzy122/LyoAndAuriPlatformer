using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
    public class QuestLogScrollingList : MonoBehaviour
    {
        [Header("Components")] [SerializeField]
        private GameObject contentParent;

        [Header("Rect Transform")] [SerializeField]
        private RectTransform scrollRectTransform;

        [SerializeField] private RectTransform contentRectTransform;

        [Header("Quest log Button")] [SerializeField]
        private GameObject questLogButtonPrefab;

        private Dictionary<string, QuestLogButton> idToButtonMap = new Dictionary<string, QuestLogButton>();

        /*
        private void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                QuestInfoSO questInfoTest = ScriptableObject.CreateInstance<QuestInfoSO>();
                questInfoTest.id = "test_" + i;
                questInfoTest.displayName = "Test " + i;
                questInfoTest.questStepPrefabs = new GameObject[0];
                Quest quest = new Quest(questInfoTest);

                QuestLogButton questLogButton = CreateButtonIfNotExists(quest,
                    () => { Debug.Log("SELECTED: " + questInfoTest.displayName); });

                if (i == 0)
                {
                    questLogButton.button.Select();
                }
            }
        }
        */

        public QuestLogButton CreateButtonIfNotExists(Quest quest, UnityAction selectAction)
        {
            QuestLogButton questLogButton = null;
            // only create the button if we havent seen this quest id before
            if (!idToButtonMap.ContainsKey(quest.info.id))
            {
                questLogButton = InstantiateQuestLogButton(quest, selectAction);
            }
            else
            {
                questLogButton = idToButtonMap[quest.info.id];
            }

            return questLogButton;
        }

        private QuestLogButton InstantiateQuestLogButton(Quest quest, UnityAction selectAction)
        {
            // create the button
            QuestLogButton questLogButton = Instantiate(
                questLogButtonPrefab,
                contentParent.transform).GetComponent<QuestLogButton>();
            questLogButton.gameObject.name = quest.info.id + "_button";
            // game object name in the scene
            RectTransform buttonRectTransform = questLogButton.GetComponent<RectTransform>();
            questLogButton.Initialize(quest.info.displayName, () =>
            {
                selectAction();
                UpdateScrolling(buttonRectTransform);
            });
            // add to map to keep track of the new button
            idToButtonMap[quest.info.id] = questLogButton;
            return questLogButton;
        }

        private void UpdateScrolling(RectTransform buttonRectTransform)
        {
            // calculate the min and max for the selected button
            float buttonYMin = Mathf.Abs(buttonRectTransform.anchoredPosition.y);
            float buttonYMax = buttonYMin + buttonRectTransform.rect.height;

            // calculate the min and max for the content area
            float contentYMin = contentRectTransform.anchoredPosition.y;
            float contentYMax = contentYMin + scrollRectTransform.rect.height;

            // handle scrolling down
            if (buttonYMax > contentYMax)
            {
                contentRectTransform.anchoredPosition = new Vector2(
                    contentRectTransform.anchoredPosition.x,
                    buttonYMax - scrollRectTransform.rect.height
                );
            }
            // handle scrolling up
            else if (buttonYMin < contentYMin)
            {
                contentRectTransform.anchoredPosition = new Vector2(
                    contentRectTransform.anchoredPosition.x,
                    buttonYMin
                );
            }
        }
    }
}