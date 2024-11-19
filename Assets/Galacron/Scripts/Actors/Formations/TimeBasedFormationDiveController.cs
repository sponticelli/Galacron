using Galacron.Paths;
using UnityEngine;

namespace Galacron.Actors
{
    public class TimeBasedFormationDiveController : AbstractFormationDiveController
    {
        [Header("Time-based Diving Settings")] 
        [SerializeField] private float minTimeAfterSpawn = 6f;
        [SerializeField] private float minTimeBetweenDives = 3f;
        [SerializeField] private float maxTimeBetweenDives = 10f;

        private float nextDiveTime;

        protected override void OnDivingEnabled()
        {
            nextDiveTime = Time.time + minTimeAfterSpawn;
        }

        protected override bool ShouldInitiateDive()
        {
            return Time.time >= nextDiveTime && HasValidMembers();
        }

        protected override void TryStartDive()
        {
            var members = registry.GetMembers();
            if (members.Count == 0) return;

            // Select random member
            var randomMember = members[Random.Range(0, members.Count)];
            
            // Create and setup path
            var path = CreateAndSetupPath(transform.position, Quaternion.identity);
            if (path == null) return;

            // Register path and start dive
            RegisterDivePath(randomMember.actor.EnemyID, path);
            randomMember.actor.DiveSetup(path.GetComponent<PathBase>());

            // Remove from formation registry
            registry.UnregisterMember(randomMember.actor, false);
        }

        protected override void OnDiveInitiated()
        {
            nextDiveTime = Time.time + Random.Range(minTimeBetweenDives, maxTimeBetweenDives);
        }
    }
}