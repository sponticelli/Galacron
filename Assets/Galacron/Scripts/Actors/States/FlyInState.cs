namespace Galacron.Actors.States
{
    public class FlyInState : ActorStateBase
    {
        public FlyInState(FormationActor actor) : base(actor) { }

        public override void Update()
        {
            UpdateFireTimer();
            HandleShooting(Actor.Target, Actor.Weapon, Actor.OnFlyInFireSettings);
            Actor.MoveToTarget.SetTarget(Actor.Formation.GetVector(Actor.EnemyID));
        }
    }
}