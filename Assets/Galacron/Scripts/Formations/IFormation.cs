namespace Galacron.Formations
{
    public interface IFormation
    {
        /// <summary>
        /// Add a member to the formation
        /// </summary>
        void AddMember(IMember member);
        
        /// <summary>
        /// Remove a member from the formation
        /// </summary>
        void RemoveMember(IMember member);
        
        void UpdateFormation();
        
        /// <summary>
        /// All the members have been assigned to the formation
        /// </summary>
        bool IsComplete();
        
        /// <summary>
        /// All the members have been defeated
        /// </summary>
        bool IsDefeated();
        
        /// <summary>
        /// Destroys the formation and removes all members
        /// </summary>
        void Disband();
    }
}