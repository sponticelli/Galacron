using Nexus.Core.ServiceLocation;
using UnityEngine;

namespace Galacron.Formations
{
    public class FormationFactory : MonoBehaviour
    {
        private IFormationService formationService;
        
        private void Start()
        {
            formationService = ServiceLocator.Instance.GetService<IFormationService>();
        }
        
        public IFormation CreateFormation(FormationConfig config)
        {
            var formationObject = new GameObject("Formation");
            var formation = formationObject.AddComponent<Formation>();
            formation.Config = config;
            
            formationService.RegisterFormation(formation);
            return formation;
        }
    }
}