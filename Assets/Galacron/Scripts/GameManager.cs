using Galacron.Score;
using Nexus.Core.ServiceLocation;
using UnityEngine;
using UnityEngine.Events;
using Nexus.Core.Rx;
using Nexus.Core.Rx.Unity;

namespace Galacron
{
    public class GameManager : ObservableMonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private UnityEvent onGameStart;

        
        private IScoreManager _scoreManager;
        
        public void Initialize()
        {
            _scoreManager = ServiceLocator.Instance.GetService<IScoreManager>();
            
            AddDisposable(
                    GameEvents.OnEnemyKilled.Subscribe(
                        enemyEvent => _scoreManager.AddPoints(enemyEvent.PointValue)
                    )
            );
            
            
            // TODO Register the event to FloatingTextManager

            Debug.Log("Game Manager Initialized");
            onGameStart.Invoke();
        }
        
       
    }
}