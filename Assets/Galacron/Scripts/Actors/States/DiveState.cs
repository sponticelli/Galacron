namespace Galacron.Actors.States
{
    public class DiveState : ActorStateBase
    {
        public DiveState(FormationActor actor) : base(actor) { }

        public override void Enter()
        {
            Actor.ActivateTrails(true);
        }

        public override void Update()
        {
            UpdateFireTimer();
            HandleShooting(Actor.Target, Actor.Weapon, Actor.OnDiveFireSettings);
        }
    }
}