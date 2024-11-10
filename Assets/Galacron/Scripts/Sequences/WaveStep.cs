using Galacron.Actors;
using Nexus.Sequences;
using UnityEngine;

namespace Galacron.Sequences
{
    public class WaveStep : BaseStep
    {
        [Header("References")]
        [SerializeField] private Spawner spawner;
        
        
        public override void StartStep()
        {
            base.StartStep();
            spawner.StartSpawning();
            Complete();
        }
        
        public void OnWaveEnd()
        {
            Finish();
        }
        
    }
}