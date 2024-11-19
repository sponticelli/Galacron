using System.Collections.Generic;

namespace Galacron.Actors
{
    public interface IFormationRegistry
    {
        void SetExpectedMembers(int count);
        void RegisterMember(FormationActor actor, int positionIndex);
        void UnregisterMember(FormationActor actor, bool destroyed = false);
        bool HasMember(FormationActor actor);
        List<FormationRegistry.FormationMember> GetMembers();
    }
}