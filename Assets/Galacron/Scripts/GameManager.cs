using Galacron.FloatingTexts;
using Galacron.Score;
using Nexus.Core.ServiceLocation;
using UnityEngine;
using UnityEngine.Events;
using Nexus.Core.Rx;
using Nexus.Core.Rx.Unity;
using Nexus.Pooling;
using Nexus.Sequences;

namespace Galacron
{
    public class GameManager : ObservableMonoBehaviour
    {
        [Header("Flòating Text")]
        [SerializeField] private PoolReference<FloatingText> _enemyDeathFloatingText;
        [SerializeField] private Color _enemyDeathFloatingTextColor;
        
        [Header("Wave Sequence")]
        [SerializeField] private Sequence _waveSequence;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onGameStart;
        
        
        private IScoreManager _scoreManager;
        private IFloatingTextFactory _floatingTextFactory;
        
        public void Initialize()
        {
            _scoreManager = ServiceLocator.Instance.GetService<IScoreManager>();
            _floatingTextFactory = ServiceLocator.Instance.GetService<IFloatingTextFactory>();
            
            AddDisposable(
                    GameEvents.OnEnemyKilled.Subscribe(
                        enemyEvent => _scoreManager.AddPoints(enemyEvent.PointValue)
                    )
                
            );
            
            AddDisposable(
                GameEvents.OnEnemyKilled.Subscribe(
                    enemyEvent =>
                    {
                        GameEvents.OnEnemyKilled.Subscribe(
                            enemyEvent =>
                            {
                                var value = $"+{enemyEvent.PointValue}";
                                _floatingTextFactory.Create(_enemyDeathFloatingText, 
                                            value,
                                            _enemyDeathFloatingTextColor, 
                                            enemyEvent.Position);
                            });
                    }
                )
            );
            
            // TODO Register the event to FloatingTextManager

            _waveSequence.Context.SetData<GameData>(new GameData());
            
            Debug.Log("Game Manager Initialized");
            
            
            onGameStart.Invoke();
        }
        
       
    }
}