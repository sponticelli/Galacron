using Nexus.Core.Rx;
using Nexus.Core.Rx.Unity;
using Nexus.Core.ServiceLocation;
using UnityEngine;

namespace Galacron.Score
{
    public class HighscoreDisplay : ObservableMonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI highscoreText;
    
        private IScoreManager scoreManager;
    
        public void Start()
        {
            scoreManager = ServiceLocator.Instance.GetService<IScoreManager>();
            var highscore = scoreManager.GetHighscore();
            UpdateDisplay(highscore);
            
            
            // Subscribe to highscore changes
            AddDisposable(
                scoreManager.Highscore
                    .Delay(0.5f)
                    .Subscribe(highscore => 
                    {
                        UpdateDisplay(highscore);
                    })
            );
            
        }
    
        private void UpdateDisplay(int highscore)
        {
            highscoreText.text = $"{highscore:N0}";
        }
    }
}