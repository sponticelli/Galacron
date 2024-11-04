using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using Nexus.Sequences;
using UnityEngine;

namespace Galacron.Formations
{
    public class InFormationSpawnFactory:  BaseSpawnFactory
    {
        [Header("References")]
        [SerializeField] private BaseSpawnFactory _spawnFactory;
        
        
        private IFormationService _formationService;
        private IMember _formationMember;

        
        private void Start()
        {
            _formationService = ServiceLocator.Instance.GetService<IFormationService>();
            if (_formationService == null)
            {
                Debug.LogError("InFormationSpawnFactory requires a formation service to be present in the service locator");
            }
        }

        public override GameObject CreateSpawnObject(Vector3 position, Quaternion rotation)
        {
            var go = _spawnFactory.CreateSpawnObject(position, rotation);
            var formationMember = go.GetComponent<IMember>();
            if (formationMember != null)
            {
                _formationService.AssignToFormation(formationMember);
                return go;
            }
            
            Debug.LogError("Spawned object does not have a formation member component");
            go.ReturnToPool();
            
            return null;
        }
    }
}