using System;
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
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var formationObject = new GameObject($"Formation_{config.name}");
            
            var formation = formationObject.AddComponent<Formation>();
            var pattern = formationObject.AddComponent<FormationPattern>();
            var movement = formationObject.AddComponent<FormationMovement>();
            
            try
            {
                formation.Initialize(config, pattern, movement);
                formationService.RegisterFormation(formation);
                Debug.Log($"Created formation: {formationObject.name}");
                return formation;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create formation: {ex.Message}");
                Destroy(formationObject);
                throw;
            }
        }
    }
}