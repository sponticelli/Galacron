using System.Collections.Generic;
using Galacron.Paths;
using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actors
{
    public abstract class AbstractFormationDiveController : MonoBehaviour, IFormationDiveController
    {
        [SerializeField] protected List<PoolReference<PathBase>> divePathList = new();
        [SerializeField] protected FormationRegistry registry;
        protected Dictionary<int, GameObject> activeDivePaths = new Dictionary<int, GameObject>();
        protected bool isDivingEnabled;

        protected virtual void Awake()
        {
            if (registry==null) registry = GetComponent<FormationRegistry>();
        }

        protected virtual void OnEnable()
        {
            activeDivePaths.Clear();
        }

        public virtual void EnableDiving()
        {
            isDivingEnabled = true;
            OnDivingEnabled();
        }

        protected virtual void Update()
        {
            if (!isDivingEnabled || !ShouldInitiateDive()) return;

            TryStartDive();
            OnDiveInitiated();
        }

        // Abstract methods that derived classes must implement
        protected abstract bool ShouldInitiateDive();
        protected abstract void TryStartDive();
        protected abstract void OnDivingEnabled();
        protected abstract void OnDiveInitiated();

        public virtual void OnDiveComplete(int actorId)
        {
            if (activeDivePaths.TryGetValue(actorId, out var pathObject) && pathObject != null)
            {
                activeDivePaths.Remove(actorId);
                if (pathObject != null && !pathObject.Equals(null))
                {
                    pathObject.ReturnToPool();
                }
            }

            OnDiveCompleted(actorId);
        }

        // Virtual method that derived classes can override to handle dive completion
        protected virtual void OnDiveCompleted(int actorId) { }

        protected virtual void OnDisable()
        {
            var pathsToCleanup = new List<GameObject>();

            foreach (var pathObj in activeDivePaths.Values)
            {
                if (pathObj != null && !pathObj.Equals(null))
                {
                    pathsToCleanup.Add(pathObj);
                }
            }

            foreach (var pathObj in pathsToCleanup)
            {
                try
                {
                    pathObj.ReturnToPool();
                }
                catch (MissingReferenceException)
                {
                    // Ignore errors for already destroyed objects
                }
            }

            activeDivePaths.Clear();
        }

        // Protected helper methods that derived classes might find useful
        protected virtual GameObject CreateAndSetupPath(Vector3 position, Quaternion rotation)
        {
            if (divePathList.Count == 0) return null;
            
            var randomPathRef = divePathList[Random.Range(0, divePathList.Count)];
            var path = randomPathRef.Get(position, rotation);
            path.RecalculatePath();
            return path.gameObject;
        }

        protected virtual void RegisterDivePath(int actorId, GameObject pathObject)
        {
            if (pathObject != null)
            {
                activeDivePaths[actorId] = pathObject;
            }
        }

        protected virtual bool HasValidMembers()
        {
            var members = registry.GetMembers();
            return members is { Count: > 0 };
        }
    }
}