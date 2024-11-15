using UnityEngine;

namespace Galacron.Actors.States
{
    public interface IActorState
    {
        void Enter();
        void Update();
        void Exit();
        void HandleShooting(Transform target, Weapon weapon, FireSettings fireSettings);
    }
}