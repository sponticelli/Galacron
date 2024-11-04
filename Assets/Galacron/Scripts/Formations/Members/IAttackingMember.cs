namespace Galacron.Formations
{
    /// <summary>
    /// Interface for formation members that can perform attacks
    /// </summary>
    public interface IAttackingMember
    {
        bool IsAttacking { get; }
        void StartAttack();
        void EndAttack();
    }
}