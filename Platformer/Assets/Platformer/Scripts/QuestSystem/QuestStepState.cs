using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    [System.Serializable]
    public class QuestStepState
    {
        public string state;
       

        public QuestStepState(string state)
        {
            this.state = state;
            
        }

        public QuestStepState()
        {
            this.state = "";
            
        }
    }
}