using Nexus.Core.Rx;
using Nexus.Core.Services;

namespace Galacron.Score
{
    [ServiceInterface]
    public interface IScoreManager
    {
        public IObservable<int> Score { get; }
        public IObservable<int> Highscore { get; }
        public void AddPoints(int points);
        public void ResetScore();
        public void ResetHighscore();
        public int GetCurrentScore();
        public int GetHighscore();
    }
}