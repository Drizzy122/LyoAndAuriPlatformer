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
                UpdateState();
            }

            if (coinsCollected >= coinsToComplete)
            {
                FinishQuestStep();
            }
        }
        
        private void UpdateState()
        {
            string state = coinsCollected.ToString();
            ChangeState(state);
        }
        
        protected override void SetQuestStepState(string state)
        {
            this.coinsCollected = System.Int32.Parse(state);
            UpdateState();
        }
    }
}