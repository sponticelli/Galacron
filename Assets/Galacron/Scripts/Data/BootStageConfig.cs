using System.Collections.Generic;
using Nexus.Core.Bootstrap;
using UnityEngine;


namespace Galacron.Data
{
    [CreateAssetMenu(menuName = "Galacron/Boot/Create BootStageConfig", fileName = "BootStageConfig")]
    public class BootStageConfig : ScriptableObject
    {
        [SerializeField] private List<BootStageDescription> _stages = new List<BootStageDescription>();
        public IReadOnlyList<BootStageDescription> Stages => _stages;
        
        public BootStageDescription GetStage(BootstrapStage stage)
        {
            return _stages.Find(s => s.Stage == stage);
        }
    }
}