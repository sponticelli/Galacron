namespace Galacron.Actors
{
    public interface IFormationDiveController
    {
        void EnableDiving();
        void OnDiveComplete(int actorId);
    }
}