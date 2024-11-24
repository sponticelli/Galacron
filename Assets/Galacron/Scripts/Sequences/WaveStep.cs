using Galacron.Actors;
using Nexus.Sequences;
using UnityEngine;

namespace Galacron.Sequences
{
    public class WaveStep : BaseStepWithContext
    {
        [Header("References")]
        [SerializeField] private Spawner spawner;
        
        
        public override void StartStep()
        {
            base.StartStep();
            spawner.gameObject.SetActive(true);
            spawner.StartSpawning();
            Complete();
        }
        
        public void OnWaveEnd()
        {
            Finish();
        }
        
    }
}