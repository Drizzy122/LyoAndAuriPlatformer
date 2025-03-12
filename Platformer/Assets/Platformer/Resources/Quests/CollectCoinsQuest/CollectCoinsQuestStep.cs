using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{ 
    public class CollectCoinsQuestStep : QuestStep
    {
        private int coinsCollected = 0;
        private int coinsToComplete = 5;

        private void OnEnable()
        {
            GameEventsManager.instance.miscEvents.OnCoinCollected += CoinCollected;
        }

        private void OnDisable()
        {
            GameEventsManager.instance.miscEvents.OnCoinCollected -= CoinCollected;
        }

        private void CoinCollected()
        {
            if (coinsCollected < coinsToComplete)
            {
                coinsCollected++;
            }

            if (coinsCollected >= coinsToComplete)
            {
                FinishQuestStep();
            }
        }
    }
}