namespace Galacron.Actors.States
{
    public class IdleState : ActorStateBase
    {
        public IdleState(FormationActor actor) : base(actor) { }

        public override void Enter()
        {
            Actor.ActivateTrails(false);
        }

        public override void Update()
        {
            UpdateFireTimer();
            HandleShooting(Actor.Target, Actor.Weapon, Actor.OnIdleFireSettings);
        }
    }
}