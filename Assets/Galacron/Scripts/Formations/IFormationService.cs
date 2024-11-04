using Nexus.Core.Services;
using UnityEngine.Serialization;

namespace Galacron.Formations
{
    [ServiceInterface]
    public interface IFormationService : IInitiable
    {
        public void RegisterFormation(IFormation formation);
        public void UnregisterFormation(IFormation formation);
        public bool AssignToFormation(IMember member);
    }
}