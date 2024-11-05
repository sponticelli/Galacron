namespace Galacron.Formations
{
    public interface IFormationMovement
    {
        void Initialize(FormationConfig config, IFormationPattern pattern);
        void UpdateMovement();
    }
}