using Nexus.Core.Rx;
using Nexus.Core.Rx.Unity;
using Nexus.Core.ServiceLocation;
using UnityEngine;

namespace Galacron.Score
{
    public class ScoreDisplay : ObservableMonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    
        private IScoreManager scoreManager;
    
        public void Start()
        {
            scoreManager = ServiceLocator.Instance.GetService<IScoreManager>();
            // Subscribe to score changes
            AddDisposable(
                scoreManager.Score
                    .Delay(0.25f)
                    .Subscribe(score => 
                    {
                        UpdateDisplay(score);
                    })
            );
            
        }
    
        private void UpdateDisplay(int score)
        {
            scoreText.text = $"{score:N0}";
        }
    }
    

}