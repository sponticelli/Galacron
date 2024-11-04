namespace Galacron.Formations
{
    public interface IMemberState
    {
        void Enter(IMember enemy);
        void Update(IMember enemy, float deltaTime);
        void Exit(IMember enemy);
    }
}