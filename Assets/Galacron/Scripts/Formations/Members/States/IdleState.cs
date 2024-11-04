using UnityEngine;

namespace Galacron.Formations
{
    /// <summary>
    /// State for idle formation behavior
    /// </summary>
    public class IdleState : IMemberState
    {
        private readonly IFormation formation;
        private readonly FormationSlot slot;
        private Vector2 basePosition;

        public IdleState(IFormation formation, FormationSlot slot)
        {
            this.formation = formation;
            this.slot = slot;
        }

        public void Enter(IMember member)
        {
            basePosition = ((Formation)formation).GetSlotWorldPosition(slot);
        }

        public void Update(IMember member, float deltaTime)
        {
            // Position is managed by formation's idle movement
        }

        public void Exit(IMember member)
        {
        }
    }
}