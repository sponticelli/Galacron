using UnityEngine;

namespace Galacron.Actors.States
{
    public abstract class ActorStateBase : IActorState
    {
        protected readonly FormationActor Actor;
        protected float NextFireTime;

        protected ActorStateBase(FormationActor actor)
        {
            Actor = actor;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }

        public virtual void HandleShooting(Transform target, Weapon weapon, FireSettings fireSettings)
        {
            if (!fireSettings.canFire || weapon == null || target == null || NextFireTime > 0)
                return;

            var direction = (target.position - Actor.transform.position).normalized;
            var precision = fireSettings.GetPrecision();
            direction += new Vector3(
                Random.Range(-precision, precision),
                Random.Range(-precision, precision),
                0
            );

            weapon.transform.up = direction;
            weapon.Shoot();
            NextFireTime = fireSettings.GetFireRate();
        }

        protected void UpdateFireTimer()
        {
            NextFireTime -= Time.deltaTime;
        }
    }
}