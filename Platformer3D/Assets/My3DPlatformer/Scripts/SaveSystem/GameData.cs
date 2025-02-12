using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    [System.Serializable]
    public class GameData
    {
        public long lastUpdated;
        public int deathCount;

        public Vector3 playerPosition;
        

        public SerializableDictionary<string, bool> coinsCollected;
        public SerializableDictionary<string, bool> ecliptiumCollected;
        public SerializableDictionary<string, bool> luminCollected;


        // the values defined in this constructor will be the default values
        // the game starts with when there's no data to load
        public GameData()
        {
            this.deathCount = 0;
            playerPosition = Vector3.zero;
            //for collectables
            coinsCollected = new SerializableDictionary<string, bool>();
            ecliptiumCollected = new SerializableDictionary<string, bool>();
            luminCollected = new SerializableDictionary<string, bool>();
        }

        public int GetPercentageComplete()
        {
            // figure out how manu coins we've collected
            int totalCollected = 0;
            ;
            foreach (bool collected in coinsCollected.Values)
            {
                if (collected)
                {
                    totalCollected++;
                }
            }

            foreach (bool collected in ecliptiumCollected.Values)
            {
                if (collected)
                {
                    totalCollected++;
                }
            }

            foreach (bool collected in luminCollected.Values)
            {
                if (collected)
                {
                    totalCollected++;
                }
            }

            // ensure we don't divide by 0 when calculating the percentage
            int percentageComplete = -1;
            if (coinsCollected.Count != 0)
            {
                percentageComplete = (totalCollected * 100 / coinsCollected.Count);
            }

            if (ecliptiumCollected.Count != 0)
            {
                percentageComplete = (totalCollected * 100 / ecliptiumCollected.Count);
            }

            if (luminCollected.Count != 0)
            {
                percentageComplete = (totalCollected * 100 / luminCollected.Count);
            }

            return percentageComplete;
        }
    }
}