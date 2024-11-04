using Nexus.Sequences;
using UnityEngine;

namespace Galacron.Formations
{
    public class FormationCreationStep : BaseStep
    {
        [SerializeField] private FormationConfig _formationConfig;
        [SerializeField] private FormationFactory _formationFactory;
        
        
        public override void StartStep()
        {
            base.StartStep();
            var formation = _formationFactory.CreateFormation(_formationConfig);
            Complete();
            Finish();
        }
    }
}