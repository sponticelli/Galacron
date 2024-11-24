using Nexus.Pooling;
using UnityEngine;
using UnityEngine.Events;

namespace Galacron.Actors
{
    public class BossWithPilot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FormationActor formationActor;
        [SerializeField] private Transform pilotSpawnPoint;
        
        
        [Header("Pilot")]
        [SerializeField] private PoolReference<Health> pilotPrefab;
        
        [Header("Events")]
        public UnityEvent onDeath;
        public UnityEvent<float> onDamage;
        
        
        
        private Health pilot;
        
        private void Awake()
        {
            if (formationActor == null)
            {
                formationActor = GetComponent<FormationActor>();
            }
        }
        
        private void OnEnable()
        {
            if (pilot == null)
            {
                pilot = pilotPrefab.Get(pilotSpawnPoint.position, Quaternion.identity);
                pilot.transform.SetParent(transform);
                pilot.onDeath.AddListener(OnPilotDeath);
            }
            else
            {
                pilot.Revive();
            }
        }

        private void OnPilotDeath()
        {
            pilot.onDeath.RemoveListener(OnPilotDeath);
            pilot = null;
            onDeath?.Invoke();
            gameObject.ReturnToPool();
        }
    }
}