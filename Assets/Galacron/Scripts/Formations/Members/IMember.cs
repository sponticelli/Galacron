using UnityEngine;

namespace Galacron.Formations
{
    public interface IMember
    {
        void SetFormationPosition(Vector2 position);
        Vector2 GetCurrentPosition();
        void OnFormationAssigned(IFormation formation);
        void SetState(IMemberState state);
    }
}