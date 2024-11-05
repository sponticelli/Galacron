using System.Collections.Generic;
using UnityEngine;

namespace Galacron.Formations
{
    public interface IFormationPattern
    {
        FormationConfig Config { get; }
        List<FormationSlot> Slots { get; }
        FormationSlot GetAvailableSlot();
        FormationSlot FindSlotByMember(IMember member);
        void AssignSlot(FormationSlot slot, IMember member);
        void ReleaseSlot(FormationSlot slot);
        Vector2 GetSlotWorldPosition(FormationSlot slot);
        bool IsComplete();
        void Initialize(FormationConfig config);
    }
}