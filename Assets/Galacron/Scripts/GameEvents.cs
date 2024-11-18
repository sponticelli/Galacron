using Galacron.Score;
using Nexus.Core.Rx;
using Nexus.Core.Rx.Unity;

namespace Galacron
{
    public static class GameEvents
    {
        public static readonly Subject<EnemyKilledEvent> OnEnemyKilled = 
            new GameEventSubject<EnemyKilledEvent>();
        
 
    }
}