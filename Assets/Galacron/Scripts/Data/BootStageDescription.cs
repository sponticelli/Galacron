using System;
using Nexus.Core.Bootstrap;
using UnityEngine;

namespace Galacron.Data
{
    [Serializable]
    public class  BootStageDescription
    {
        [SerializeField] private BootstrapStage _stage;
        [SerializeField] private string  _stateName;
        [SerializeField] private string[] _catchPhrases;
        
        public BootstrapStage Stage => _stage;
        public string StateName => _stateName;
        public string[] CatchPhrases => _catchPhrases;
    }
}