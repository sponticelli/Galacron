
using Nexus.Core.Rx;
using Nexus.Core.Rx.Unity;
using Nexus.Core.Services;

namespace Galacron.Score
{
    [ServiceImplementation]
    public class ScoreManager : IScoreManager
    {
        private readonly ReactiveProperty<int> scoreProperty;
        private readonly ReactiveProperty<int> highScoreProperty;
    
        public ScoreManager()
        {
            highScoreProperty = new GameProperty<int>(0, "HighScore");
            scoreProperty = new ReactiveProperty<int>(0);
        }
    
        public IObservable<int> Score => scoreProperty;
        public IObservable<int> Highscore => highScoreProperty;
    
        public void AddPoints(int points)
        {
            scoreProperty.Value += points;
            
            if (scoreProperty.Value > highScoreProperty.Value)
            {
                highScoreProperty.Value = scoreProperty.Value;
            }
        }
    
        public void ResetScore()
        {
            scoreProperty.Value = 0;
        }
        
        public void ResetHighscore()
        {
            highScoreProperty.Value = 0;
        }
    
        public int GetCurrentScore() => scoreProperty.Value;
        public int GetHighscore() => highScoreProperty.Value;
    }
    
}