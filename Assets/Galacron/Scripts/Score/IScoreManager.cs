using Nexus.Core.Rx;
using Nexus.Core.Services;

namespace Galacron.Score
{
    [ServiceInterface]
    public interface IScoreManager
    {
        public IObservable<int> Score { get; }
        public IObservable<int> HighScore { get; }
        public void AddPoints(int points);
        public void ResetScore();
        public void ResetHighScore();
        public int GetCurrentScore();
        public int GetHighScore();
    }
}