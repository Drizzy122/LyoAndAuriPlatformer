namespace Platformer
{
    public class CollectCoinsQuestStep : QuestStep
    {
        private int coinsCollected = 0;
        private int coinsToComplete = 5;

        private void Start()
        {
            UpdateState();
        }
        private void OnEnable()
        {
            GameEventsManager.instance.miscEvents.onLuminCollected += CoinCollected;
          
        }

        private void OnDisable()
        {
            GameEventsManager.instance.miscEvents.onLuminCollected -= CoinCollected;
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
            string status = "Collected" + coinsCollected + " / " + coinsToComplete + "coins.";
            ChangeState(state, status);
        }
        
        protected override void SetQuestStepState(string state)
        {
            this.coinsCollected = System.Int32.Parse(state);
            UpdateState();
        }
    }
}