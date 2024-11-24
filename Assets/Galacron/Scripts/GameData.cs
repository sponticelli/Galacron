using System;
using Nexus.Sequences;

namespace Galacron
{
    [Serializable]
    public class GameData : IStepData
    {
        private int currentLevel = 0;
        private int numberOfRestarts = 0;
        
        
        public int CurrentLevel => currentLevel;
        public int NumberOfRestarts => numberOfRestarts;
        
        public GameData()
        {
            Reset();
        }
        
        public void Reset()
        {
            currentLevel = 0;
            numberOfRestarts = 0;
        }
        
        public void IncrementLevel()
        {
            currentLevel++;
        }
        
        public void IncrementRestarts()
        {
            numberOfRestarts++;
        }
    }
}